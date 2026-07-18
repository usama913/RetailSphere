using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Contracts.Common;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.GetProducts;

public sealed record GetProductsQuery(
    int Page,
    int PageSize,
    string? Search,
    long? CategoryId,
    long? BrandId,
    bool? IsActive) : IRequest<Result<PagedResult<ProductDto>>>;
