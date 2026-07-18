using RetailSphere.Contracts.Catalog;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<IReadOnlyList<CategoryDto>>> GetCategoriesAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(long id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> ActivateCategoryAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeactivateCategoryAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeleteCategoryAsync(long id, CancellationToken cancellationToken = default);
}
