using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Sales;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<PagedResult<SalesOrderDto>>> GetSalesOrdersAsync(
        int page = 1,
        int pageSize = 25,
        string? search = null,
        long? branchId = null,
        long? customerId = null,
        string? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<SalesOrderDto>> GetSalesOrderByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<SalesOrderDto>> CreateSalesOrderAsync(CreateSalesOrderRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<SalesOrderDto>> CancelSalesOrderAsync(long id, CancelSalesOrderRequest request, CancellationToken cancellationToken = default);
}
