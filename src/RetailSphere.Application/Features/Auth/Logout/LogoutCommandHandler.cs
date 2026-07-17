using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Auth.Logout;

public sealed class LogoutCommandHandler(
    IUserRepository userRepository,
    IJwtTokenService tokenService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var hash = tokenService.HashRefreshToken(request.RefreshToken);
        var user = await userRepository.GetByRefreshTokenHashAsync(hash, cancellationToken);

        // Logging out with an already-invalid token is not an error — the end
        // state the caller wants (no active session for this token) is already true.
        if (user is null)
            return Result.Success();

        var token = user.RefreshTokens.FirstOrDefault(t => t.TokenHash == hash);
        if (token is not null && token.IsActive)
        {
            user.RevokeRefreshTokenFamily(token, "User logged out.");
            userRepository.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
