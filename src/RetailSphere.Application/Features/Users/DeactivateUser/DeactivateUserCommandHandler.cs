using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Users.DeactivateUser;

public sealed class DeactivateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<DeactivateUserCommand, Result>
{
    public async Task<Result> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
            return Result.Failure(Error.NotFound("User.NotFound", "User not found."));

        user.Deactivate();
        userRepository.Update(user);
        auditLogService.Log("User", user.Id.ToString(), "Deactivated", $"Deactivated user '{user.Email.Value}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
