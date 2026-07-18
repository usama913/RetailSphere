using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Customers;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<IReadOnlyList<CustomerDto>>> GetCustomersAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    Task<ApiResponse<CustomerDto>> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<CustomerDto>> UpdateCustomerAsync(long id, UpdateCustomerRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> ActivateCustomerAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeactivateCustomerAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeleteCustomerAsync(long id, CancellationToken cancellationToken = default);
}
