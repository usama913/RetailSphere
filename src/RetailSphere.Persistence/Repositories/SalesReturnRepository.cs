using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Sales;

namespace RetailSphere.Persistence.Repositories;

public sealed class SalesReturnRepository(RetailSphereDbContext dbContext) : ISalesReturnRepository
{
    public Task<SalesReturn?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.SalesReturns
            .Include(r => r.Lines)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<SalesReturn> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        long? branchId,
        long? customerId,
        long? salesOrderId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.SalesReturns
            .Include(r => r.Lines)
            .AsQueryable();

        if (branchId.HasValue)
            query = query.Where(r => r.BranchId == branchId.Value);

        if (customerId.HasValue)
            query = query.Where(r => r.CustomerId == customerId.Value);

        if (salesOrderId.HasValue)
            query = query.Where(r => r.SalesOrderId == salesOrderId.Value);

        if (fromDate.HasValue)
            query = query.Where(r => r.ReturnDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(r => r.ReturnDate <= toDate.Value);

        query = query.OrderByDescending(r => r.ReturnDate);

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<bool> ReturnNumberExistsAsync(string returnNumber, CancellationToken cancellationToken = default) =>
        dbContext.SalesReturns.IgnoreQueryFilters().AnyAsync(r => r.ReturnNumber == returnNumber, cancellationToken);

    public void Add(SalesReturn salesReturn) => dbContext.SalesReturns.Add(salesReturn);

    public void Remove(SalesReturn salesReturn) => dbContext.SalesReturns.Remove(salesReturn);
}
