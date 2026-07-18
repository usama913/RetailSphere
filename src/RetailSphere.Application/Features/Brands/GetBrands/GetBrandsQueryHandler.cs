using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Brands.GetBrands;

public sealed class GetBrandsQueryHandler(IBrandRepository brandRepository)
    : IRequestHandler<GetBrandsQuery, Result<IReadOnlyList<BrandDto>>>
{
    public async Task<Result<IReadOnlyList<BrandDto>>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
    {
        var brands = await brandRepository.GetAllAsync(request.IncludeInactive, cancellationToken);
        var dtos = brands.Select(BrandMappings.ToDto).ToList();

        return Result.Success<IReadOnlyList<BrandDto>>(dtos);
    }
}
