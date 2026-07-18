using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Admin;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Roles.CreateRole;

public sealed class CreateRoleCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<CreateRoleCommand, Result<RoleDto>>
{
    public async Task<Result<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (await roleRepository.NameExistsAsync(request.Name.Trim(), cancellationToken: cancellationToken))
            return Result.Failure<RoleDto>(Error.Conflict("Role.DuplicateName", "A role with this name already exists."));

        var roleResult = Role.Create(request.Name, request.Description);
        if (roleResult.IsFailure)
            return Result.Failure<RoleDto>(roleResult.Error);

        var role = roleResult.Value;
        foreach (var permissionId in request.PermissionIds.Distinct())
        {
            role.GrantPermission(permissionId);
        }

        roleRepository.Add(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        auditLogService.Log("Role", role.Id.ToString(), "Created", $"Created role '{role.Name}' with {role.PermissionIds.Count} permission(s).");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(RoleMappings.ToDto(role));
    }
}
