namespace RetailSphere.Domain.Catalog;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Category>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    Task<bool> HasChildrenAsync(long id, CancellationToken cancellationToken = default);

    void Add(Category category);

    void Update(Category category);

    /// <summary>Soft-deleted by the audit interceptor, same as Role.Remove — see AuditableEntitySaveChangesInterceptor.</summary>
    void Remove(Category category);
}
