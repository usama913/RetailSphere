using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.ActivateProduct;

public sealed record ActivateProductCommand(long Id) : IRequest<Result>;
