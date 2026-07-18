using MediatR;
using RetailSphere.Application.Features.Products.Common;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Contracts.Common;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.GetProducts;

public sealed class GetProductsQueryHandler(IProductRepository productRepository, ProductDtoAssembler productDtoAssembler)
    : IRequestHandler<GetProductsQuery, Result<PagedResult<ProductDto>>>
{
    public async Task<Result<PagedResult<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await productRepository.SearchAsync(
            request.Page, request.PageSize, request.Search, request.CategoryId, request.BrandId, request.IsActive, cancellationToken);

        var dtos = await productDtoAssembler.ToDtosAsync(items, cancellationToken);

        return Result.Success(new PagedResult<ProductDto>
        {
            Data = dtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        });
    }
}
