using RetailSphere.Domain.IdentityAccess;

namespace RetailSphere.Application.Common.Services;

/// <summary>
/// Resolves a user's effective role names and flattened permission codes.
/// Shared by Login and RefreshToken handlers so token contents are always
/// assembled the same way.
/// </summary>
public sealed class UserClaimsResolver(IRoleRepository roleRepository)
{
    public async Task<(IReadOnlyList<string> RoleNames, IReadOnlyList<string> PermissionCodes)> ResolveAsync(
        User user, CancellationToken cancellationToken = default)
    {
        var roleNames = new List<string>();
        var permissionCodes = new HashSet<string>();

        foreach (var roleId in user.RoleIds)
        {
            var role = await roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role is null) continue;

            roleNames.Add(role.Name);

            var permissions = await roleRepository.GetPermissionsAsync(role.PermissionIds, cancellationToken);
            foreach (var permission in permissions)
                permissionCodes.Add(permission.Code);
        }

        return (roleNames, permissionCodes.ToList());
    }
}
