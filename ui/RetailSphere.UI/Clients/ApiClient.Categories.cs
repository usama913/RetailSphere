using RetailSphere.Contracts.Catalog;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<IReadOnlyList<CategoryDto>>> GetCategoriesAsync(bool includeInactive = false, CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<CategoryDto>>($"categories?includeInactive={includeInactive}", cancellationToken);

    public Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateCategoryRequest, CategoryDto>("categories", request, cancellationToken);

    public Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(long id, UpdateCategoryRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateCategoryRequest, CategoryDto>($"categories/{id}", request, cancellationToken);

    public Task<ApiResponse<object>> ActivateCategoryAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<object>($"categories/{id}/activate", cancellationToken);

    public Task<ApiResponse<object>> DeactivateCategoryAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<object>($"categories/{id}/deactivate", cancellationToken);

    public Task<ApiResponse<object>> DeleteCategoryAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync<object>($"categories/{id}", cancellationToken);
}
