using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.UpdateProduct;

public sealed record UpdateProductCommand(
    long Id,
    string Name,
    string? Description,
    long? CategoryId,
    long? BrandId,
    long? UnitId,
    bool ManageStock,
    bool NotForSelling) : IRequest<Result<ProductDto>>;
