using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.ProductAttributes.UpdateProductAttribute;

public sealed record UpdateProductAttributeCommand(long Id, string Name) : IRequest<Result<ProductAttributeDto>>;
