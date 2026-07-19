using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Finance;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<PagedResult<ExpenseDto>>> GetExpensesAsync(
        int page = 1,
        int pageSize = 25,
        long? branchId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? category = null,
        CancellationToken cancellationToken = default);

    /// <summary>Suggested defaults merged with every distinct category already recorded — see GetExpenseCategoriesQueryHandler.</summary>
    Task<ApiResponse<IReadOnlyList<string>>> GetExpenseCategoriesAsync(CancellationToken cancellationToken = default);

    Task<ApiResponse<ExpenseDto>> CreateExpenseAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<ExpenseDto>> UpdateExpenseAsync(long id, UpdateExpenseRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeleteExpenseAsync(long id, CancellationToken cancellationToken = default);
}
