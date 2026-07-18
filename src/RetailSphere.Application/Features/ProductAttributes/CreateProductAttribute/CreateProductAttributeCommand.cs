using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.ProductAttributes.CreateProductAttribute;

public sealed record CreateProductAttributeCommand(string Name) : IRequest<Result<ProductAttributeDto>>;
