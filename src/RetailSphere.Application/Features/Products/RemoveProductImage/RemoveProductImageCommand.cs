using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.RemoveProductImage;

public sealed record RemoveProductImageCommand(long ProductId, long ImageId) : IRequest<Result<ProductDto>>;
