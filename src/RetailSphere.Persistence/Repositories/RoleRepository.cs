using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.IdentityAccess;

namespace RetailSphere.Persistence.Repositories;

public sealed class RoleRepository(RetailSphereDbContext dbContext) : IRoleRepository
{
    public Task<Role?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Roles.ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Permission>> GetPermissionsAsync(IEnumerable<long> permissionIds, CancellationToken cancellationToken = default)
    {
        var ids = permissionIds.ToList();
        if (ids.Count == 0) return [];

        return await dbContext.Permissions
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetAllPermissionsAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Permissions
            .OrderBy(p => p.Module)
            .ThenBy(p => p.DisplayName)
            .ToListAsync(cancellationToken);

    public void Add(Role role) => dbContext.Roles.Add(role);

    public void Update(Role role) => dbContext.Roles.Update(role);

    // A hard Remove() here is converted into a soft delete (IsDeleted = true) by
    // AuditableEntitySaveChangesInterceptor, since Role implements ISoftDeletable.
    public void Remove(Role role) => dbContext.Roles.Remove(role);
}
