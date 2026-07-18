using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.ProductAttributes.DeleteProductAttribute;

public sealed record DeleteProductAttributeCommand(long Id) : IRequest<Result>;
