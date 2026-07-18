using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.GetProductById;

public sealed record GetProductByIdQuery(long Id) : IRequest<Result<ProductDto>>;
