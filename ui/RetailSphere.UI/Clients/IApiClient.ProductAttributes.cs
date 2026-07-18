using RetailSphere.Contracts.Catalog;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<IReadOnlyList<ProductAttributeDto>>> GetProductAttributesAsync(CancellationToken cancellationToken = default);

    Task<ApiResponse<ProductAttributeDto>> CreateProductAttributeAsync(CreateProductAttributeRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<ProductAttributeDto>> UpdateProductAttributeAsync(long id, UpdateProductAttributeRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeleteProductAttributeAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<ProductAttributeDto>> AddAttributeValueAsync(long id, AddAttributeValueRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<ProductAttributeDto>> RemoveAttributeValueAsync(long id, long valueId, CancellationToken cancellationToken = default);
}
