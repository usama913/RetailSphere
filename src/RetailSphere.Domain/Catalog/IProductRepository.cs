namespace RetailSphere.Domain.Catalog;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Batch-resolves the owning Products (with Variants loaded) for a set of
    /// ProductVariantIds — the lookup the Inventory module needs to show a SKU/Product
    /// name for a StockItem, since StockItem only stores a plain ProductVariantId and
    /// there's no dedicated ProductVariant repository (it's a child of Product).
    /// </summary>
    Task<IReadOnlyList<Product>> GetByVariantIdsAsync(IEnumerable<long> variantIds, CancellationToken cancellationToken = default);

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
