using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Finance;

namespace RetailSphere.Persistence.Repositories;

public sealed class ExpenseRepository(RetailSphereDbContext dbContext) : IExpenseRepository
{
    public Task<Expense?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Expenses.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Expense> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        long? branchId,
        DateTime? fromDate,
        DateTime? toDate,
        string? category,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Expenses.AsQueryable();

        if (branchId.HasValue)
            query = query.Where(e => e.BranchId == branchId.Value);

        if (fromDate.HasValue)
            query = query.Where(e => e.ExpenseDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(e => e.ExpenseDate <= toDate.Value);

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(e => e.Category == category);

        query = query.OrderByDescending(e => e.ExpenseDate);

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<string>> GetDistinctCategoriesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Expenses
            .Select(e => e.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);

    public void Add(Expense expense) => dbContext.Expenses.Add(expense);

    public void Update(Expense expense) => dbContext.Expenses.Update(expense);

    public void Remove(Expense expense) => dbContext.Expenses.Remove(expense);
}
