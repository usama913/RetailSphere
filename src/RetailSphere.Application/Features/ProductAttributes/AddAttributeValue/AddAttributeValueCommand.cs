using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.ProductAttributes.AddAttributeValue;

public sealed record AddAttributeValueCommand(long ProductAttributeId, string Value) : IRequest<Result<ProductAttributeDto>>;
