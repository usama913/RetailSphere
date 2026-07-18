namespace RetailSphere.Domain.Catalog;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Product> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        string? search,
        long? categoryId,
        long? brandId,
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default);

    void Add(Product product);

    void Update(Product product);

    void Remove(Product product);
}
