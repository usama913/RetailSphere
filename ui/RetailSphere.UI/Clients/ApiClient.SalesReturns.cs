using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Sales;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<PagedResult<SalesReturnDto>>> GetSalesReturnsAsync(
        int page = 1,
        int pageSize = 25,
        long? branchId = null,
        long? customerId = null,
        long? salesOrderId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = $"page={page}&pageSize={pageSize}";

        if (branchId.HasValue)
            query += $"&branchId={branchId.Value}";

        if (customerId.HasValue)
            query += $"&customerId={customerId.Value}";

        if (salesOrderId.HasValue)
            query += $"&salesOrderId={salesOrderId.Value}";

        if (fromDate.HasValue)
            query += $"&fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}";

        if (toDate.HasValue)
            query += $"&toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}";

        return GetAsync<PagedResult<SalesReturnDto>>($"sales-returns?{query}", cancellationToken);
    }

    public Task<ApiResponse<SalesReturnDto>> GetSalesReturnByIdAsync(long id, CancellationToken cancellationToken = default) =>
        GetAsync<SalesReturnDto>($"sales-returns/{id}", cancellationToken);

    public Task<ApiResponse<SalesReturnDto>> CreateSalesReturnAsync(CreateSalesReturnRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateSalesReturnRequest, SalesReturnDto>("sales-returns", request, cancellationToken);
}
