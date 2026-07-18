using RetailSphere.Contracts.Catalog;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<IReadOnlyList<ProductAttributeDto>>> GetProductAttributesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<ProductAttributeDto>>("product-attributes", cancellationToken);

    public Task<ApiResponse<ProductAttributeDto>> CreateProductAttributeAsync(CreateProductAttributeRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateProductAttributeRequest, ProductAttributeDto>("product-attributes", request, cancellationToken);

    public Task<ApiResponse<ProductAttributeDto>> UpdateProductAttributeAsync(long id, UpdateProductAttributeRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateProductAttributeRequest, ProductAttributeDto>($"product-attributes/{id}", request, cancellationToken);

    public Task<ApiResponse<object>> DeleteProductAttributeAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync<object>($"product-attributes/{id}", cancellationToken);

    public Task<ApiResponse<ProductAttributeDto>> AddAttributeValueAsync(long id, AddAttributeValueRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<AddAttributeValueRequest, ProductAttributeDto>($"product-attributes/{id}/values", request, cancellationToken);

    public Task<ApiResponse<ProductAttributeDto>> RemoveAttributeValueAsync(long id, long valueId, CancellationToken cancellationToken = default) =>
        DeleteAsync<ProductAttributeDto>($"product-attributes/{id}/values/{valueId}", cancellationToken);
}
