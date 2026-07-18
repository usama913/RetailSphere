using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Inventory;

namespace RetailSphere.Persistence.Repositories;

public sealed class StockTransferRepository(RetailSphereDbContext dbContext) : IStockTransferRepository
{
    public Task<StockTransfer?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.StockTransfers
            .Include(t => t.Lines)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<StockTransfer> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        string? search,
        long? fromBranchId,
        long? toBranchId,
        string? status,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.StockTransfers
            .Include(t => t.Lines)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(t => t.TransferNumber.ToLower().Contains(term));
        }

        if (fromBranchId.HasValue)
            query = query.Where(t => t.FromBranchId == fromBranchId.Value);

        if (toBranchId.HasValue)
            query = query.Where(t => t.ToBranchId == toBranchId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(t => t.Status == status);

        query = query.OrderByDescending(t => t.TransferDate);

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <summary>
    /// Ignores the soft-delete query filter from day one — see
    /// PurchaseOrderRepository.PoNumberExistsAsync for why a naive filtered check
    /// would let a soft-deleted transfer's number look free forever while its
    /// physical row (and the unique index on TransferNumber) still blocks reuse.
    /// </summary>
    public Task<bool> TransferNumberExistsAsync(string transferNumber, CancellationToken cancellationToken = default) =>
        dbContext.StockTransfers.IgnoreQueryFilters().AnyAsync(t => t.TransferNumber == transferNumber, cancellationToken);

    public void Add(StockTransfer stockTransfer) => dbContext.StockTransfers.Add(stockTransfer);

    public void Update(StockTransfer stockTransfer) => dbContext.StockTransfers.Update(stockTransfer);

    public void Remove(StockTransfer stockTransfer) => dbContext.StockTransfers.Remove(stockTransfer);
}
