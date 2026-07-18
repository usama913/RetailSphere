namespace RetailSphere.Domain.Inventory;

public interface IStockItemRepository
{
    Task<StockItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<StockItem?> GetByVariantAndBranchAsync(long productVariantId, long branchId, CancellationToken cancellationToken = default);

    /// <summary>
    /// productId filters to StockItems whose variant belongs to that Product — resolved
    /// via a plain join against ProductVariants, not an EF navigation (StockItem has no
    /// relationship to Catalog; see the class remarks on StockItem).
    /// </summary>
    Task<(IReadOnlyList<StockItem> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        long? branchId,
        long? productId,
        CancellationToken cancellationToken = default);

    void Add(StockItem stockItem);

    void Update(StockItem stockItem);
}
