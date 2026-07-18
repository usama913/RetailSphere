using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Roles.DeleteRole;

public sealed class DeleteRoleCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<DeleteRoleCommand, Result>
{
    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (role is null)
            return Result.Failure(Error.NotFound("Role.NotFound", "Role not found."));

        if (role.IsSystemRole)
            return Result.Failure(Error.Failure("Role.SystemRoleImmutable", "System roles cannot be deleted."));

        roleRepository.Remove(role);
        auditLogService.Log("Role", role.Id.ToString(), "Deleted", $"Deleted role '{role.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
