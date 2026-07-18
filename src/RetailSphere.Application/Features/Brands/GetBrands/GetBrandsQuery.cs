using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Brands.GetBrands;

public sealed record GetBrandsQuery(bool IncludeInactive) : IRequest<Result<IReadOnlyList<BrandDto>>>;
