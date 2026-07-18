using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.RemoveVariant;

public sealed record RemoveVariantCommand(long ProductId, long VariantId) : IRequest<Result<ProductDto>>;
