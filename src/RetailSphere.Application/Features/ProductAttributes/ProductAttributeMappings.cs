using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;

namespace RetailSphere.Application.Features.ProductAttributes;

internal static class ProductAttributeMappings
{
    public static ProductAttributeDto ToDto(ProductAttribute attribute) => new()
    {
        Id = attribute.Id,
        Name = attribute.Name,
        Values = attribute.Values
            .OrderBy(v => v.DisplayOrder)
            .Select(ToDto)
            .ToList(),
    };

    public static AttributeValueDto ToDto(AttributeValue value) => new()
    {
        Id = value.Id,
        Value = value.Value,
        DisplayOrder = value.DisplayOrder,
    };
}
