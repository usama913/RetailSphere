using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.Domain.IdentityAccess.ValueObjects;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Users.ResetPassword;

public sealed class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<ResetPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
            return Result.Failure(Error.NotFound("User.NotFound", "User not found."));

        var hashedPasswordResult = HashedPassword.FromHash(passwordHasher.Hash(request.NewPassword));
        if (hashedPasswordResult.IsFailure)
            return Result.Failure(hashedPasswordResult.Error);

        user.ChangePassword(hashedPasswordResult.Value);
        userRepository.Update(user);
        auditLogService.Log("User", user.Id.ToString(), "PasswordReset", $"Reset password for '{user.Email.Value}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
