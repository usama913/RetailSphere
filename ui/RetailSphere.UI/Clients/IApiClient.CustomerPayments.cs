using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Customers;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<PagedResult<CustomerPaymentDto>>> GetCustomerPaymentsAsync(
        int page = 1,
        int pageSize = 25,
        long? customerId = null,
        long? branchId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<CustomerPaymentDto>> GetCustomerPaymentByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<CustomerPaymentDto>> RecordCustomerPaymentAsync(RecordCustomerPaymentRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<CustomerPaymentDto>> UpdateCustomerPaymentAsync(long id, UpdateCustomerPaymentRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<CustomerPaymentDto>> ReverseCustomerPaymentAsync(long id, ReverseCustomerPaymentRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<CustomerPaymentDto>> AllocateCustomerPaymentAsync(long id, AllocateCustomerPaymentRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<CustomerPaymentDto>> RemoveCustomerPaymentAllocationAsync(long id, long salesOrderId, CancellationToken cancellationToken = default);
}
