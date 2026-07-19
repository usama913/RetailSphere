using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Finance;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<PagedResult<ExpenseDto>>> GetExpensesAsync(
        int page = 1,
        int pageSize = 25,
        long? branchId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? category = null,
        CancellationToken cancellationToken = default)
    {
        var query = $"page={page}&pageSize={pageSize}";

        if (branchId.HasValue)
            query += $"&branchId={branchId.Value}";

        if (fromDate.HasValue)
            query += $"&fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}";

        if (toDate.HasValue)
            query += $"&toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}";

        if (!string.IsNullOrWhiteSpace(category))
            query += $"&category={Uri.EscapeDataString(category)}";

        return GetAsync<PagedResult<ExpenseDto>>($"expenses?{query}", cancellationToken);
    }

    public Task<ApiResponse<IReadOnlyList<string>>> GetExpenseCategoriesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<string>>("expenses/categories", cancellationToken);

    public Task<ApiResponse<ExpenseDto>> CreateExpenseAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateExpenseRequest, ExpenseDto>("expenses", request, cancellationToken);

    public Task<ApiResponse<ExpenseDto>> UpdateExpenseAsync(long id, UpdateExpenseRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateExpenseRequest, ExpenseDto>($"expenses/{id}", request, cancellationToken);

    public Task<ApiResponse<object>> DeleteExpenseAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync<object>($"expenses/{id}", cancellationToken);
}
