using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Sales;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<PagedResult<SalesOrderDto>>> GetSalesOrdersAsync(
        int page = 1,
        int pageSize = 25,
        string? search = null,
        long? branchId = null,
        long? customerId = null,
        string? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = $"page={page}&pageSize={pageSize}";

        if (!string.IsNullOrWhiteSpace(search))
            query += $"&search={Uri.EscapeDataString(search)}";

        if (branchId.HasValue)
            query += $"&branchId={branchId.Value}";

        if (customerId.HasValue)
            query += $"&customerId={customerId.Value}";

        if (!string.IsNullOrWhiteSpace(status))
            query += $"&status={Uri.EscapeDataString(status)}";

        if (fromDate.HasValue)
            query += $"&fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}";

        if (toDate.HasValue)
            query += $"&toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}";

        return GetAsync<PagedResult<SalesOrderDto>>($"sales-orders?{query}", cancellationToken);
    }

    public Task<ApiResponse<SalesOrderDto>> GetSalesOrderByIdAsync(long id, CancellationToken cancellationToken = default) =>
        GetAsync<SalesOrderDto>($"sales-orders/{id}", cancellationToken);

    public Task<ApiResponse<SalesOrderDto>> CreateSalesOrderAsync(CreateSalesOrderRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateSalesOrderRequest, SalesOrderDto>("sales-orders", request, cancellationToken);

    public Task<ApiResponse<SalesOrderDto>> CancelSalesOrderAsync(long id, CancelSalesOrderRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CancelSalesOrderRequest, SalesOrderDto>($"sales-orders/{id}/cancel", request, cancellationToken);
}
