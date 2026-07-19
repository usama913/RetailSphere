using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Sales;

namespace RetailSphere.Persistence.Repositories;

public sealed class SalesOrderRepository(RetailSphereDbContext dbContext) : ISalesOrderRepository
{
    public Task<SalesOrder?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.SalesOrders
            .Include(s => s.Lines)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<SalesOrder> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        string? search,
        long? branchId,
        long? customerId,
        string? status,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.SalesOrders
            .Include(s => s.Lines)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(s => s.OrderNumber.ToLower().Contains(term));
        }

        if (branchId.HasValue)
            query = query.Where(s => s.BranchId == branchId.Value);

        if (customerId.HasValue)
            query = query.Where(s => s.CustomerId == customerId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(s => s.Status == status);

        if (fromDate.HasValue)
            query = query.Where(s => s.OrderDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(s => s.OrderDate <= toDate.Value);

        query = query.OrderByDescending(s => s.OrderDate);

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<SalesOrder>> GetByCustomerAsync(long customerId, CancellationToken cancellationToken = default) =>
        await dbContext.SalesOrders
            .Include(s => s.Lines)
            .Where(s => s.CustomerId == customerId)
            .OrderBy(s => s.OrderDate)
            .ToListAsync(cancellationToken);

    /// <summary>
    /// OutstandingBalance is computed (not a mapped column), so filtering by it can't
    /// be translated into SQL — pulled into memory (scoped to one customer, so the
    /// row count stays small) the same way PurchaseInvoiceRepository.SearchAsync
    /// handles its PaymentStatus filter.
    /// </summary>
    public async Task<IReadOnlyList<SalesOrder>> GetOutstandingByCustomerAsync(long customerId, CancellationToken cancellationToken = default)
    {
        var orders = await dbContext.SalesOrders
            .Include(s => s.Lines)
            .Where(s => s.CustomerId == customerId && s.Status != "Cancelled")
            .ToListAsync(cancellationToken);

        return orders.Where(s => s.OutstandingBalance > 0).OrderBy(s => s.DueDate ?? s.OrderDate).ToList();
    }

    public async Task<IReadOnlyList<SalesOrder>> GetOutstandingAsync(CancellationToken cancellationToken = default)
    {
        var orders = await dbContext.SalesOrders
            .Where(s => s.CustomerId != null && s.Status != "Cancelled")
            .ToListAsync(cancellationToken);

        return orders.Where(s => s.OutstandingBalance > 0).ToList();
    }

    /// <summary>
    /// Ignores the soft-delete query filter — same reasoning as
    /// PurchaseOrderRepository.PoNumberExistsAsync: Remove() never actually deletes a
    /// row (the audit interceptor turns it into IsDeleted = true), so a "deleted"
    /// order's number would otherwise look free forever to the generator.
    /// </summary>
    public Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default) =>
        dbContext.SalesOrders.IgnoreQueryFilters().AnyAsync(s => s.OrderNumber == orderNumber, cancellationToken);

    public void Add(SalesOrder salesOrder) => dbContext.SalesOrders.Add(salesOrder);

    public void Update(SalesOrder salesOrder) => dbContext.SalesOrders.Update(salesOrder);

    public void Remove(SalesOrder salesOrder) => dbContext.SalesOrders.Remove(salesOrder);
}
