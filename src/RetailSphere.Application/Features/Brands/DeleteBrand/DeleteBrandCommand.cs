using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Brands.DeleteBrand;

public sealed record DeleteBrandCommand(long Id) : IRequest<Result>;
