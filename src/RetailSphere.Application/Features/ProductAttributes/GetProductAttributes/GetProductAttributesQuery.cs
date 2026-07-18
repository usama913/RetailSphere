using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.ProductAttributes.GetProductAttributes;

public sealed record GetProductAttributesQuery : IRequest<Result<IReadOnlyList<ProductAttributeDto>>>;
