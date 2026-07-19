using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Customers;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<IReadOnlyList<CustomerDto>>> GetCustomersAsync(bool includeInactive = false, CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<CustomerDto>>($"customers?includeInactive={includeInactive}", cancellationToken);

    public Task<ApiResponse<CustomerDto>> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateCustomerRequest, CustomerDto>("customers", request, cancellationToken);

    public Task<ApiResponse<CustomerDto>> UpdateCustomerAsync(long id, UpdateCustomerRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateCustomerRequest, CustomerDto>($"customers/{id}", request, cancellationToken);

    public Task<ApiResponse<CustomerDto>> UpdateCustomerCreditLimitAsync(long id, UpdateCustomerCreditLimitRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateCustomerCreditLimitRequest, CustomerDto>($"customers/{id}/credit-limit", request, cancellationToken);

    public Task<ApiResponse<object>> ActivateCustomerAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<object>($"customers/{id}/activate", cancellationToken);

    public Task<ApiResponse<object>> DeactivateCustomerAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<object>($"customers/{id}/deactivate", cancellationToken);

    public Task<ApiResponse<object>> DeleteCustomerAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync<object>($"customers/{id}", cancellationToken);
}
