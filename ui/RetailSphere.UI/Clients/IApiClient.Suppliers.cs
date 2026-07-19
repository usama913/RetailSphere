using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Purchasing;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<IReadOnlyList<SupplierDto>>> GetSuppliersAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    Task<ApiResponse<SupplierDto>> CreateSupplierAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<SupplierDto>> UpdateSupplierAsync(long id, UpdateSupplierRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<SupplierDto>> UpdateSupplierCreditTermsAsync(long id, UpdateSupplierCreditTermsRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> ActivateSupplierAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeactivateSupplierAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeleteSupplierAsync(long id, CancellationToken cancellationToken = default);
}
