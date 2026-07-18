namespace RetailSphere.Domain.Catalog;

public interface IProductAttributeRepository
{
    Task<ProductAttribute?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductAttribute>> GetAllAsync(CancellationToken cancellationToken = default);

    void Add(ProductAttribute attribute);

    void Update(ProductAttribute attribute);

    void Remove(ProductAttribute attribute);
}
