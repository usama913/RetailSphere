using RetailSphere.Contracts.Catalog;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<IReadOnlyList<BrandDto>>> GetBrandsAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    Task<ApiResponse<BrandDto>> CreateBrandAsync(CreateBrandRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<BrandDto>> UpdateBrandAsync(long id, UpdateBrandRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> ActivateBrandAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeactivateBrandAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeleteBrandAsync(long id, CancellationToken cancellationToken = default);
}
