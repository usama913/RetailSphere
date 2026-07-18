using RetailSphere.Contracts.Catalog;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<IReadOnlyList<BrandDto>>> GetBrandsAsync(bool includeInactive = false, CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<BrandDto>>($"brands?includeInactive={includeInactive}", cancellationToken);

    public Task<ApiResponse<BrandDto>> CreateBrandAsync(CreateBrandRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateBrandRequest, BrandDto>("brands", request, cancellationToken);

    public Task<ApiResponse<BrandDto>> UpdateBrandAsync(long id, UpdateBrandRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateBrandRequest, BrandDto>($"brands/{id}", request, cancellationToken);

    public Task<ApiResponse<object>> ActivateBrandAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<object>($"brands/{id}/activate", cancellationToken);

    public Task<ApiResponse<object>> DeactivateBrandAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<object>($"brands/{id}/deactivate", cancellationToken);

    public Task<ApiResponse<object>> DeleteBrandAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync<object>($"brands/{id}", cancellationToken);
}
