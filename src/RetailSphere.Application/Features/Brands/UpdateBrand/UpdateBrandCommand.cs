using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Brands.UpdateBrand;

public sealed record UpdateBrandCommand(long Id, string Name, string? Description) : IRequest<Result<BrandDto>>;
