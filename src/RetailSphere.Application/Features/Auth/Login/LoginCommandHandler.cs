using MediatR;
using Microsoft.Extensions.Options;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Options;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Auth;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Auth.Login;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    UserClaimsResolver claimsResolver,
    IPasswordHasher passwordHasher,
    IJwtTokenService tokenService,
    IUnitOfWork unitOfWork,
    IOptions<AuthOptions> authOptions)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        // Deliberately identical error for "not found" and "wrong password" —
        // never reveal which part of the credential pair was wrong.
        if (user is null || !passwordHasher.Verify(request.Password, user.Password.Value))
            return Result.Failure<LoginResponse>(Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password."));

        if (!user.IsActive)
            return Result.Failure<LoginResponse>(Error.Unauthorized("Auth.UserInactive", "This account has been deactivated."));

        var (roleNames, permissionCodes) = await claimsResolver.ResolveAsync(user, cancellationToken);

        var (accessToken, accessTokenExpiresAtUtc) = tokenService.GenerateAccessToken(
            user.Id, user.Email.Value, roleNames, permissionCodes, user.BranchId);

        var rawRefreshToken = tokenService.GenerateRefreshToken();
        var refreshTokenHash = tokenService.HashRefreshToken(rawRefreshToken);
        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(authOptions.Value.RefreshTokenLifetimeDays);

        user.IssueRefreshToken(refreshTokenHash, refreshTokenExpiresAtUtc, request.ClientIp);
        user.RecordLogin();

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new LoginResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = accessTokenExpiresAtUtc,
            RefreshToken = rawRefreshToken,
            UserId = user.Id,
            FullName = $"{user.FirstName} {user.LastName}",
            BranchId = user.BranchId,
            Roles = roleNames,
            Permissions = permissionCodes,
        });
    }
}
