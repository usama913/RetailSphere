using RetailSphere.Application.Features.Products;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;

namespace RetailSphere.Application.Features.Products.Common;

/// <summary>Resolves the CategoryName/BrandName/UnitName lookups ProductDto needs on top of Product's own scalar fields — mirrors UserDtoAssembler.</summary>
public sealed class ProductDtoAssembler(ICategoryRepository categoryRepository, IBrandRepository brandRepository, IUnitRepository unitRepository)
{
    public async Task<ProductDto> ToDtoAsync(Product product, CancellationToken cancellationToken = default)
    {
        string? categoryName = null;
        if (product.CategoryId.HasValue)
        {
            var category = await categoryRepository.GetByIdAsync(product.CategoryId.Value, cancellationToken);
            categoryName = category?.Name;
        }

        string? brandName = null;
        if (product.BrandId.HasValue)
        {
            var brand = await brandRepository.GetByIdAsync(product.BrandId.Value, cancellationToken);
            brandName = brand?.Name;
        }

        string? unitName = null;
        if (product.UnitId.HasValue)
        {
            var unit = await unitRepository.GetByIdAsync(product.UnitId.Value, cancellationToken);
            unitName = unit?.Name;
        }

        return ProductMappings.ToDto(product, categoryName, brandName, unitName);
    }

    public async Task<IReadOnlyList<ProductDto>> ToDtosAsync(IEnumerable<Product> products, CancellationToken cancellationToken = default)
    {
        var categories = (await categoryRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(c => c.Id, c => c.Name);
        var brands = (await brandRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(b => b.Id, b => b.Name);
        var units = (await unitRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(u => u.Id, u => u.Name);

        return products
            .Select(product => ProductMappings.ToDto(
                product,
                product.CategoryId.HasValue && categories.TryGetValue(product.CategoryId.Value, out var categoryName) ? categoryName : null,
                product.BrandId.HasValue && brands.TryGetValue(product.BrandId.Value, out var brandName) ? brandName : null,
                product.UnitId.HasValue && units.TryGetValue(product.UnitId.Value, out var unitName) ? unitName : null))
            .ToList();
    }
}
