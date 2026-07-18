using MediatR;
using RetailSphere.Application.Features.Products.Common;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.GetProductById;

public sealed class GetProductByIdQueryHandler(IProductRepository productRepository, ProductDtoAssembler productDtoAssembler)
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result.Failure<ProductDto>(Error.NotFound("Product.NotFound", "Product not found."));

        var dto = await productDtoAssembler.ToDtoAsync(product, cancellationToken);
        return Result.Success(dto);
    }
}
