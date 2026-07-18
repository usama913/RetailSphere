using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Brands.ActivateBrand;

public sealed record ActivateBrandCommand(long Id) : IRequest<Result>;
