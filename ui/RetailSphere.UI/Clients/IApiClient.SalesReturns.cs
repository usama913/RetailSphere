using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Sales;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<PagedResult<SalesReturnDto>>> GetSalesReturnsAsync(
        int page = 1,
        int pageSize = 25,
        long? branchId = null,
        long? customerId = null,
        long? salesOrderId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<SalesReturnDto>> GetSalesReturnByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<SalesReturnDto>> CreateSalesReturnAsync(CreateSalesReturnRequest request, CancellationToken cancellationToken = default);
}
