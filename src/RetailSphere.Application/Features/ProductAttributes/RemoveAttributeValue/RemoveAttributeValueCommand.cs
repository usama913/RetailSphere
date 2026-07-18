using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.ProductAttributes.RemoveAttributeValue;

public sealed record RemoveAttributeValueCommand(long ProductAttributeId, long AttributeValueId) : IRequest<Result<ProductAttributeDto>>;
