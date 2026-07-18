using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Brands.DeactivateBrand;

public sealed record DeactivateBrandCommand(long Id) : IRequest<Result>;
