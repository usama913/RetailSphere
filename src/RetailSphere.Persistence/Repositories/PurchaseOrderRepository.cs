using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Purchasing;

namespace RetailSphere.Persistence.Repositories;

public sealed class PurchaseOrderRepository(RetailSphereDbContext dbContext) : IPurchaseOrderRepository
{
    public Task<PurchaseOrder?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.PurchaseOrders
            .Include(p => p.Lines)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<PurchaseOrder> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        string? search,
        long? supplierId,
        long? branchId,
        string? status,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.PurchaseOrders
            .Include(p => p.Lines)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(p => p.PoNumber.ToLower().Contains(term));
        }

        if (supplierId.HasValue)
            query = query.Where(p => p.SupplierId == supplierId.Value);

        if (branchId.HasValue)
            query = query.Where(p => p.BranchId == branchId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(p => p.Status == status);

        query = query.OrderByDescending(p => p.OrderDate);

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <summary>
    /// Deliberately ignores the soft-delete query filter: PurchaseOrders has
    /// HasQueryFilter(p => !p.IsDeleted), but Remove() never actually deletes a row
    /// (AuditableEntitySaveChangesInterceptor turns it into IsDeleted = true), so the
    /// unique index on PoNumber still holds the value for a "deleted" order forever.
    /// If this check obeyed the filter, a deleted order's number would look free,
    /// the generator would try to reuse it, and every retry would hit the same
    /// MySQL duplicate-key error — which is exactly what was happening.
    /// </summary>
    public Task<bool> PoNumberExistsAsync(string poNumber, CancellationToken cancellationToken = default) =>
        dbContext.PurchaseOrders.IgnoreQueryFilters().AnyAsync(p => p.PoNumber == poNumber, cancellationToken);

    public void Add(PurchaseOrder purchaseOrder) => dbContext.PurchaseOrders.Add(purchaseOrder);

    public void Update(PurchaseOrder purchaseOrder) => dbContext.PurchaseOrders.Update(purchaseOrder);

    public void Remove(PurchaseOrder purchaseOrder) => dbContext.PurchaseOrders.Remove(purchaseOrder);
}
