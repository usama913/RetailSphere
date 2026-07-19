using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Purchasing;

namespace RetailSphere.Persistence.Repositories;

public sealed class SupplierPaymentRepository(RetailSphereDbContext dbContext) : ISupplierPaymentRepository
{
    public Task<SupplierPayment?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.SupplierPayments.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<SupplierPayment> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        long? supplierId,
        long? purchaseInvoiceId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.SupplierPayments.AsQueryable();

        if (supplierId.HasValue)
            query = query.Where(p => p.SupplierId == supplierId.Value);

        if (purchaseInvoiceId.HasValue)
            query = query.Where(p => p.PurchaseInvoiceId == purchaseInvoiceId.Value);

        if (fromDate.HasValue)
            query = query.Where(p => p.PaymentDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(p => p.PaymentDate <= toDate.Value);

        query = query.OrderByDescending(p => p.PaymentDate);

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<SupplierPayment>> GetBySupplierAsync(long supplierId, CancellationToken cancellationToken = default) =>
        await dbContext.SupplierPayments
            .Where(p => p.SupplierId == supplierId)
            .OrderBy(p => p.PaymentDate)
            .ToListAsync(cancellationToken);

    public void Add(SupplierPayment payment) => dbContext.SupplierPayments.Add(payment);

    public void Update(SupplierPayment payment) => dbContext.SupplierPayments.Update(payment);
}
