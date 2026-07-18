using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.ProductAttributes.GetProductAttributes;

public sealed class GetProductAttributesQueryHandler(IProductAttributeRepository productAttributeRepository)
    : IRequestHandler<GetProductAttributesQuery, Result<IReadOnlyList<ProductAttributeDto>>>
{
    public async Task<Result<IReadOnlyList<ProductAttributeDto>>> Handle(GetProductAttributesQuery request, CancellationToken cancellationToken)
    {
        var attributes = await productAttributeRepository.GetAllAsync(cancellationToken);
        var dtos = attributes.Select(ProductAttributeMappings.ToDto).ToList();

        return Result.Success<IReadOnlyList<ProductAttributeDto>>(dtos);
    }
}
