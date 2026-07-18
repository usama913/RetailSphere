using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.AddProductImage;

public sealed record AddProductImageCommand(long ProductId, string Url) : IRequest<Result<ProductDto>>;
