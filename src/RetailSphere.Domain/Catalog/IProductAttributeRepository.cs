namespace RetailSphere.Domain.Catalog;

public interface IProductAttributeRepository
{
    Task<ProductAttribute?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductAttribute>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Ignores the soft-delete filter — see ProductAttributeRepository.NameExistsAsync.</summary>
    Task<bool> NameExistsAsync(string name, long? excludeId = null, CancellationToken cancellationToken = default);

    void Add(ProductAttribute attribute);

    void Update(ProductAttribute attribute);

    void Remove(ProductAttribute attribute);
}
