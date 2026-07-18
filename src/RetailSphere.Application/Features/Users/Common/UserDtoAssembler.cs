using RetailSphere.Application.Features.Users;
using RetailSphere.Contracts.Admin;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.Domain.Organization;

namespace RetailSphere.Application.Features.Users.Common;

/// <summary>
/// Resolves the lookups (branch name, role names) that UserDto needs on top of
/// the User aggregate's own scalar fields — shared by every Users query/command
/// handler that returns a UserDto so the branch/role name resolution logic
/// (and its two round trips through IBranchRepository/IRoleRepository) lives in
/// exactly one place.
/// </summary>
public sealed class UserDtoAssembler(IBranchRepository branchRepository, IRoleRepository roleRepository)
{
    public async Task<UserDto> ToDtoAsync(User user, CancellationToken cancellationToken = default)
    {
        string? branchName = null;
        if (user.BranchId.HasValue)
        {
            var branch = await branchRepository.GetByIdAsync(user.BranchId.Value, cancellationToken);
            branchName = branch?.Name;
        }

        var roleNames = new List<string>();
        foreach (var roleId in user.RoleIds)
        {
            var role = await roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role is not null)
                roleNames.Add(role.Name);
        }

        return UserMappings.ToDto(user, branchName, roleNames);
    }

    public async Task<IReadOnlyList<UserDto>> ToDtosAsync(IEnumerable<User> users, CancellationToken cancellationToken = default)
    {
        // Batch-load branches/roles once rather than per-user, then map in memory.
        var branches = (await branchRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(b => b.Id, b => b.Name);
        var roles = (await roleRepository.GetAllAsync(cancellationToken))
            .ToDictionary(r => r.Id, r => r.Name);

        return users
            .Select(user => UserMappings.ToDto(
                user,
                user.BranchId.HasValue && branches.TryGetValue(user.BranchId.Value, out var branchName) ? branchName : null,
                user.RoleIds.Where(roles.ContainsKey).Select(id => roles[id]).ToList()))
            .ToList();
    }
}
