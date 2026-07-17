using MediatR;
using Microsoft.Extensions.Options;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Options;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Auth;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Auth;

public sealed class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    UserClaimsResolver claimsResolver,
    IJwtTokenService tokenService,
    IUnitOfWork unitOfWork,
    IOptions<AuthOptions> authOptions)
    : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var incomingHash = tokenService.HashRefreshToken(request.RefreshToken);

        var user = await userRepository.GetByRefreshTokenHashAsync(incomingHash, cancellationToken);
        if (user is null)
            return Result.Failure<LoginResponse>(Error.Unauthorized("Auth.InvalidRefreshToken", "Refresh token is invalid."));

        var token = user.RefreshTokens.FirstOrDefault(t => t.TokenHash == incomingHash);
        if (token is null)
            return Result.Failure<LoginResponse>(Error.Unauthorized("Auth.InvalidRefreshToken", "Refresh token is invalid."));

        if (token.IsRevoked)
        {
            // Reuse of an already-rotated token — treat as a compromised token
            // family and revoke every active token for this user (§7 of the
            // architecture doc: rotation + reuse detection).
            user.RevokeRefreshTokenFamily(token, "Reuse of a revoked refresh token was detected.");
            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Failure<LoginResponse>(
                Error.Unauthorized("Auth.RefreshTokenReuseDetected", "This session has been revoked for security reasons. Please log in again."));
        }

        if (token.IsExpired)
            return Result.Failure<LoginResponse>(Error.Unauthorized("Auth.RefreshTokenExpired", "Refresh token has expired."));

        var (roleNames, permissionCodes) = await claimsResolver.ResolveAsync(user, cancellationToken);

        var (accessToken, accessTokenExpiresAtUtc) = tokenService.GenerateAccessToken(
            user.Id, user.Email.Value, roleNames, permissionCodes, user.BranchId);

        var newRawRefreshToken = tokenService.GenerateRefreshToken();
        var newRefreshTokenHash = tokenService.HashRefreshToken(newRawRefreshToken);
        var newExpiresAtUtc = DateTime.UtcNow.AddDays(authOptions.Value.RefreshTokenLifetimeDays);

        user.RotateRefreshToken(token, newRefreshTokenHash, newExpiresAtUtc, request.ClientIp);

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new LoginResponse
        {
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = accessTokenExpiresAtUtc,
            RefreshToken = newRawRefreshToken,
            UserId = user.Id,
            FullName = $"{user.FirstName} {user.LastName}",
            BranchId = user.BranchId,
            Roles = roleNames,
            Permissions = permissionCodes,
        });
    }
}
