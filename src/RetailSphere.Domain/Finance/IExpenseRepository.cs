namespace RetailSphere.Domain.Finance;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Expense> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        long? branchId,
        DateTime? fromDate,
        DateTime? toDate,
        string? category,
        CancellationToken cancellationToken = default);

    /// <summary>Distinct, non-blank Category values actually recorded so far (used to seed the "Add Expense" category dropdown with real, previously-used names alongside the suggested defaults).</summary>
    Task<IReadOnlyList<string>> GetDistinctCategoriesAsync(CancellationToken cancellationToken = default);

    void Add(Expense expense);

    void Update(Expense expense);

    void Remove(Expense expense);
}
