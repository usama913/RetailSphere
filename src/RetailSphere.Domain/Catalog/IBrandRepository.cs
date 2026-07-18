namespace RetailSphere.Domain.Catalog;

public interface IBrandRepository
{
    Task<Brand?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Brand>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>Ignores the soft-delete filter — see BrandRepository.NameExistsAsync.</summary>
    Task<bool> NameExistsAsync(string name, long? excludeId = null, CancellationToken cancellationToken = default);

    void Add(Brand brand);

    void Update(Brand brand);

    void Remove(Brand brand);
}
