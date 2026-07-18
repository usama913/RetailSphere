using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.DeactivateProduct;

public sealed record DeactivateProductCommand(long Id) : IRequest<Result>;
