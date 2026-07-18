using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Brands.CreateBrand;

public sealed record CreateBrandCommand(string Name, string? Description) : IRequest<Result<BrandDto>>;
