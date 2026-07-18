using RetailSphere.Contracts.Catalog;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<PagedResult<ProductDto>>> GetProductsAsync(
        int page = 1,
        int pageSize = 25,
        string? search = null,
        long? categoryId = null,
        long? brandId = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<ProductDto>> GetProductByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<ProductDto>> UpdateProductAsync(long id, UpdateProductRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> ActivateProductAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeactivateProductAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeleteProductAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<ProductDto>> AddVariantAsync(long productId, AddVariantRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<ProductDto>> UpdateVariantAsync(long productId, long variantId, UpdateVariantRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<ProductDto>> RemoveVariantAsync(long productId, long variantId, CancellationToken cancellationToken = default);

    Task<ApiResponse<ProductDto>> ActivateVariantAsync(long productId, long variantId, CancellationToken cancellationToken = default);

    Task<ApiResponse<ProductDto>> DeactivateVariantAsync(long productId, long variantId, CancellationToken cancellationToken = default);

    Task<ApiResponse<ProductDto>> AddProductImageAsync(long productId, AddProductImageRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<ProductDto>> RemoveProductImageAsync(long productId, long imageId, CancellationToken cancellationToken = default);
}
