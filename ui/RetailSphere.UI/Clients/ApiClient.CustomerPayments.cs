using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Customers;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<PagedResult<CustomerPaymentDto>>> GetCustomerPaymentsAsync(
        int page = 1,
        int pageSize = 25,
        long? customerId = null,
        long? branchId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = $"page={page}&pageSize={pageSize}";

        if (customerId.HasValue)
            query += $"&customerId={customerId.Value}";

        if (branchId.HasValue)
            query += $"&branchId={branchId.Value}";

        if (fromDate.HasValue)
            query += $"&fromDate={fromDate.Value:yyyy-MM-dd}";

        if (toDate.HasValue)
            query += $"&toDate={toDate.Value:yyyy-MM-dd}";

        return GetAsync<PagedResult<CustomerPaymentDto>>($"customer-payments?{query}", cancellationToken);
    }

    public Task<ApiResponse<CustomerPaymentDto>> GetCustomerPaymentByIdAsync(long id, CancellationToken cancellationToken = default) =>
        GetAsync<CustomerPaymentDto>($"customer-payments/{id}", cancellationToken);

    public Task<ApiResponse<CustomerPaymentDto>> RecordCustomerPaymentAsync(RecordCustomerPaymentRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<RecordCustomerPaymentRequest, CustomerPaymentDto>("customer-payments", request, cancellationToken);

    public Task<ApiResponse<CustomerPaymentDto>> UpdateCustomerPaymentAsync(long id, UpdateCustomerPaymentRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateCustomerPaymentRequest, CustomerPaymentDto>($"customer-payments/{id}", request, cancellationToken);

    public Task<ApiResponse<CustomerPaymentDto>> ReverseCustomerPaymentAsync(long id, ReverseCustomerPaymentRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<ReverseCustomerPaymentRequest, CustomerPaymentDto>($"customer-payments/{id}/reverse", request, cancellationToken);

    public Task<ApiResponse<CustomerPaymentDto>> AllocateCustomerPaymentAsync(long id, AllocateCustomerPaymentRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<AllocateCustomerPaymentRequest, CustomerPaymentDto>($"customer-payments/{id}/allocations", request, cancellationToken);

    public Task<ApiResponse<CustomerPaymentDto>> RemoveCustomerPaymentAllocationAsync(long id, long salesOrderId, CancellationToken cancellationToken = default) =>
        DeleteAsync<CustomerPaymentDto>($"customer-payments/{id}/allocations/{salesOrderId}", cancellationToken);
}
