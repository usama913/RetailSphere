using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Admin;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Roles.UpdateRole;

public sealed class UpdateRoleCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<UpdateRoleCommand, Result<RoleDto>>
{
    public async Task<Result<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (role is null)
            return Result.Failure<RoleDto>(Error.NotFound("Role.NotFound", "Role not found."));

        var updateResult = role.UpdateDetails(request.Name, request.Description);
        if (updateResult.IsFailure)
            return Result.Failure<RoleDto>(updateResult.Error);

        var requestedIds = request.PermissionIds.Distinct().ToHashSet();
        var currentIds = role.PermissionIds.ToHashSet();

        foreach (var idToGrant in requestedIds.Except(currentIds))
        {
            role.GrantPermission(idToGrant);
        }

        foreach (var idToRevoke in currentIds.Except(requestedIds))
        {
            role.RevokePermission(idToRevoke);
        }

        roleRepository.Update(role);
        auditLogService.Log("Role", role.Id.ToString(), "Updated", $"Updated role '{role.Name}' — now has {role.PermissionIds.Count} permission(s).");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(RoleMappings.ToDto(role));
    }
}
