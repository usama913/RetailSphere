namespace RetailSphere.Domain.IdentityAccess;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Permission>> GetPermissionsAsync(IEnumerable<long> permissionIds, CancellationToken cancellationToken = default);

    /// <summary>Every seeded permission — powers the Admin &gt; Roles permission matrix (Phase 1).</summary>
    Task<IReadOnlyList<Permission>> GetAllPermissionsAsync(CancellationToken cancellationToken = default);

    void Add(Role role);

    void Update(Role role);

    /// <summary>Soft-deletes the role (converted from a hard delete by the audit interceptor — see AuditableEntitySaveChangesInterceptor).</summary>
    void Remove(Role role);
}
