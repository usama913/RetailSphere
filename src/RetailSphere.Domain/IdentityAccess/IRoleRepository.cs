namespace RetailSphere.Domain.IdentityAccess;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Permission>> GetPermissionsAsync(IEnumerable<long> permissionIds, CancellationToken cancellationToken = default);

    void Add(Role role);

    void Update(Role role);
}
