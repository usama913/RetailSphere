using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Products.DeleteProduct;

public sealed record DeleteProductCommand(long Id) : IRequest<Result>;
