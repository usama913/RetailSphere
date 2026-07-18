using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;

namespace RetailSphere.Application.Features.Categories;

internal static class CategoryMappings
{
    public static CategoryDto ToDto(Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Slug = category.Slug,
        ParentCategoryId = category.ParentCategoryId,
        IsActive = category.IsActive,
    };
}
