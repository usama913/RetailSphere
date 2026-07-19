using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Purchasing;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<IReadOnlyList<SupplierDto>>> GetSuppliersAsync(bool includeInactive = false, CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<SupplierDto>>($"suppliers?includeInactive={includeInactive}", cancellationToken);

    public Task<ApiResponse<SupplierDto>> CreateSupplierAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateSupplierRequest, SupplierDto>("suppliers", request, cancellationToken);

    public Task<ApiResponse<SupplierDto>> UpdateSupplierAsync(long id, UpdateSupplierRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateSupplierRequest, SupplierDto>($"suppliers/{id}", request, cancellationToken);

    public Task<ApiResponse<SupplierDto>> UpdateSupplierCreditTermsAsync(long id, UpdateSupplierCreditTermsRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateSupplierCreditTermsRequest, SupplierDto>($"suppliers/{id}/credit-terms", request, cancellationToken);

    public Task<ApiResponse<object>> ActivateSupplierAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<object>($"suppliers/{id}/activate", cancellationToken);

    public Task<ApiResponse<object>> DeactivateSupplierAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<object>($"suppliers/{id}/deactivate", cancellationToken);

    public Task<ApiResponse<object>> DeleteSupplierAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync<object>($"suppliers/{id}", cancellationToken);
}
