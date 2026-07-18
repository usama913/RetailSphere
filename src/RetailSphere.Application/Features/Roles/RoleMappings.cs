using RetailSphere.Contracts.Admin;
using RetailSphere.Domain.IdentityAccess;

namespace RetailSphere.Application.Features.Roles;

internal static class RoleMappings
{
    public static RoleDto ToDto(Role role) => new()
    {
        Id = role.Id,
        Name = role.Name,
        Description = role.Description,
        IsSystemRole = role.IsSystemRole,
        PermissionIds = role.PermissionIds.ToList(),
    };

    public static PermissionDto ToDto(Permission permission) => new()
    {
        Id = permission.Id,
        Code = permission.Code,
        DisplayName = permission.DisplayName,
        Module = permission.Module,
    };
}
