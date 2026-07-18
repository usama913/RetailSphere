using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.SetVariantActive;

public sealed record SetVariantActiveCommand(long ProductId, long VariantId, bool IsActive) : IRequest<Result<ProductDto>>;
