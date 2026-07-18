using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;

namespace RetailSphere.Application.Features.Brands;

internal static class BrandMappings
{
    public static BrandDto ToDto(Brand brand) => new()
    {
        Id = brand.Id,
        Name = brand.Name,
        Description = brand.Description,
        IsActive = brand.IsActive,
    };
}
