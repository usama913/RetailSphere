using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Purchasing;

namespace RetailSphere.Persistence.Repositories;

public sealed class PurchaseInvoiceRepository(RetailSphereDbContext dbContext) : IPurchaseInvoiceRepository
{
    public Task<PurchaseInvoice?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.PurchaseInvoices.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<PurchaseInvoice> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        long? supplierId,
        long? branchId,
        string? paymentStatus,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.PurchaseInvoices.AsQueryable();

        if (supplierId.HasValue)
            query = query.Where(i => i.SupplierId == supplierId.Value);

        if (branchId.HasValue)
            query = query.Where(i => i.BranchId == branchId.Value);

        if (fromDate.HasValue)
            query = query.Where(i => i.InvoiceDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(i => i.InvoiceDate <= toDate.Value);

        query = query.OrderByDescending(i => i.InvoiceDate);

        // PaymentStatus is computed (not a mapped column), so it can't be translated
        // into SQL — evaluated in memory after paging would be wrong (a page could
        // come back short), so when a status filter is requested we materialize the
        // date/supplier/branch-filtered set first and page over that in memory instead.
        if (!string.IsNullOrWhiteSpace(paymentStatus))
        {
            var all = await query.ToListAsync(cancellationToken);
            var filtered = all.Where(i => i.PaymentStatus == paymentStatus).ToList();
            var total = filtered.Count;
            var page1 = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return (page1, total);
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<PurchaseInvoice>> GetBySupplierAsync(long supplierId, CancellationToken cancellationToken = default) =>
        await dbContext.PurchaseInvoices
            .Where(i => i.SupplierId == supplierId)
            .OrderBy(i => i.InvoiceDate)
            .ToListAsync(cancellationToken);

    /// <summary>OutstandingBalance is computed (not a mapped column), so it can't be translated into SQL — narrow by supplier first (when given), materialize, then filter/order in memory. Feeds the Aging Report and the "Total Supplier Outstanding" dashboard widget.</summary>
    public async Task<IReadOnlyList<PurchaseInvoice>> GetOutstandingAsync(long? supplierId = null, CancellationToken cancellationToken = default)
    {
        var query = dbContext.PurchaseInvoices.AsQueryable();

        if (supplierId.HasValue)
            query = query.Where(i => i.SupplierId == supplierId.Value);

        var all = await query.ToListAsync(cancellationToken);

        return all
            .Where(i => i.OutstandingBalance > 0)
            .OrderBy(i => i.DueDate)
            .ToList();
    }

    public Task<bool> SupplierInvoiceNumberExistsAsync(long supplierId, string supplierInvoiceNumber, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = dbContext.PurchaseInvoices.IgnoreQueryFilters()
            .Where(i => i.SupplierId == supplierId && i.SupplierInvoiceNumber == supplierInvoiceNumber);

        if (excludeId.HasValue)
            query = query.Where(i => i.Id != excludeId.Value);

        return query.AnyAsync(cancellationToken);
    }

    public void Add(PurchaseInvoice invoice) => dbContext.PurchaseInvoices.Add(invoice);

    public void Update(PurchaseInvoice invoice) => dbContext.PurchaseInvoices.Update(invoice);

    public void Remove(PurchaseInvoice invoice) => dbContext.PurchaseInvoices.Remove(invoice);
}
