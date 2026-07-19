using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Customers;

namespace RetailSphere.Persistence.Repositories;

public sealed class CustomerPaymentRepository(RetailSphereDbContext dbContext) : ICustomerPaymentRepository
{
    public Task<CustomerPayment?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.CustomerPayments
            .Include(p => p.Allocations)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<CustomerPayment> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        long? customerId,
        long? branchId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.CustomerPayments
            .Include(p => p.Allocations)
            .AsQueryable();

        if (customerId.HasValue)
            query = query.Where(p => p.CustomerId == customerId.Value);

        if (branchId.HasValue)
            query = query.Where(p => p.BranchId == branchId.Value);

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

    public async Task<IReadOnlyList<CustomerPayment>> GetByCustomerAsync(long customerId, CancellationToken cancellationToken = default) =>
        await dbContext.CustomerPayments
            .Include(p => p.Allocations)
            .Where(p => p.CustomerId == customerId)
            .OrderBy(p => p.PaymentDate)
            .ToListAsync(cancellationToken);

    public void Add(CustomerPayment payment) => dbContext.CustomerPayments.Add(payment);

    public void Update(CustomerPayment payment) => dbContext.CustomerPayments.Update(payment);
}
