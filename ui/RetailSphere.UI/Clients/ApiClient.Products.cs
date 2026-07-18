using RetailSphere.Contracts.Catalog;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<PagedResult<ProductDto>>> GetProductsAsync(
        int page = 1,
        int pageSize = 25,
        string? search = null,
        long? categoryId = null,
        long? brandId = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = $"page={page}&pageSize={pageSize}";

        if (!string.IsNullOrWhiteSpace(search))
            query += $"&search={Uri.EscapeDataString(search)}";

        if (categoryId.HasValue)
            query += $"&categoryId={categoryId.Value}";

        if (brandId.HasValue)
            query += $"&brandId={brandId.Value}";

        if (isActive.HasValue)
            query += $"&isActive={isActive.Value}";

        return GetAsync<PagedResult<ProductDto>>($"products?{query}", cancellationToken);
    }

    public Task<ApiResponse<ProductDto>> GetProductByIdAsync(long id, CancellationToken cancellationToken = default) =>
        GetAsync<ProductDto>($"products/{id}", cancellationToken);

    public Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateProductRequest, ProductDto>("products", request, cancellationToken);

    public Task<ApiResponse<ProductDto>> UpdateProductAsync(long id, UpdateProductRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateProductRequest, ProductDto>($"products/{id}", request, cancellationToken);

    public Task<ApiResponse<object>> ActivateProductAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<object>($"products/{id}/activate", cancellationToken);

    public Task<ApiResponse<object>> DeactivateProductAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<object>($"products/{id}/deactivate", cancellationToken);

    public Task<ApiResponse<object>> DeleteProductAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync<object>($"products/{id}", cancellationToken);

    public Task<ApiResponse<ProductDto>> AddVariantAsync(long productId, AddVariantRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<AddVariantRequest, ProductDto>($"products/{productId}/variants", request, cancellationToken);

    public Task<ApiResponse<ProductDto>> UpdateVariantAsync(long productId, long variantId, UpdateVariantRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateVariantRequest, ProductDto>($"products/{productId}/variants/{variantId}", request, cancellationToken);

    public Task<ApiResponse<ProductDto>> RemoveVariantAsync(long productId, long variantId, CancellationToken cancellationToken = default) =>
        DeleteAsync<ProductDto>($"products/{productId}/variants/{variantId}", cancellationToken);

    public Task<ApiResponse<ProductDto>> ActivateVariantAsync(long productId, long variantId, CancellationToken cancellationToken = default) =>
        PostAsync<ProductDto>($"products/{productId}/variants/{variantId}/activate", cancellationToken);

    public Task<ApiResponse<ProductDto>> DeactivateVariantAsync(long productId, long variantId, CancellationToken cancellationToken = default) =>
        PostAsync<ProductDto>($"products/{productId}/variants/{variantId}/deactivate", cancellationToken);

    public Task<ApiResponse<ProductDto>> AddProductImageAsync(long productId, AddProductImageRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<AddProductImageRequest, ProductDto>($"products/{productId}/images", request, cancellationToken);

    public Task<ApiResponse<ProductDto>> RemoveProductImageAsync(long productId, long imageId, CancellationToken cancellationToken = default) =>
        DeleteAsync<ProductDto>($"products/{productId}/images/{imageId}", cancellationToken);
}
