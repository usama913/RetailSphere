using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;

namespace RetailSphere.Application.Features.Units;

internal static class UnitMappings
{
    public static UnitDto ToDto(Unit unit) => new()
    {
        Id = unit.Id,
        Name = unit.Name,
        ShortCode = unit.ShortCode,
        AllowDecimal = unit.AllowDecimal,
        IsActive = unit.IsActive,
    };
}
