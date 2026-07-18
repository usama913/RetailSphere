using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Catalog;
using RetailSphere.Domain.Inventory;

namespace RetailSphere.Persistence.Repositories;

public sealed class StockItemRepository(RetailSphereDbContext dbContext) : IStockItemRepository
{
    public Task<StockItem?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.StockItems
            .Include(s => s.Adjustments)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public Task<StockItem?> GetByVariantAndBranchAsync(long productVariantId, long branchId, CancellationToken cancellationToken = default) =>
        dbContext.StockItems
            .Include(s => s.Adjustments)
            .FirstOrDefaultAsync(s => s.ProductVariantId == productVariantId && s.BranchId == branchId, cancellationToken);

    public async Task<(IReadOnlyList<StockItem> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        long? branchId,
        long? productId,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.StockItems.Include(s => s.Adjustments).AsQueryable();

        if (branchId.HasValue)
            query = query.Where(s => s.BranchId == branchId.Value);

        if (productId.HasValue)
        {
            // No EF navigation from StockItem to ProductVariant/Product (see class
            // remarks on StockItem) — resolved as a plain subquery over the
            // ProductVariants table instead.
            var variantIds = dbContext.Set<ProductVariant>()
                .Where(v => v.ProductId == productId.Value)
                .Select(v => v.Id);
            query = query.Where(s => variantIds.Contains(s.ProductVariantId));
        }

        query = query.OrderBy(s => s.ProductVariantId).ThenBy(s => s.BranchId);

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(StockItem stockItem) => dbContext.StockItems.Add(stockItem);

    public void Update(StockItem stockItem) => dbContext.StockItems.Update(stockItem);
}
