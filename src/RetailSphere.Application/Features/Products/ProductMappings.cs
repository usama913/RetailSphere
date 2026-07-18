using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;

namespace RetailSphere.Application.Features.Products;

internal static class ProductMappings
{
    public static ProductDto ToDto(Product product, string? categoryName, string? brandName, string? unitName) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        CategoryId = product.CategoryId,
        CategoryName = categoryName,
        BrandId = product.BrandId,
        BrandName = brandName,
        UnitId = product.UnitId,
        UnitName = unitName,
        ManageStock = product.ManageStock,
        NotForSelling = product.NotForSelling,
        IsActive = product.IsActive,
        Variants = product.Variants.Select(ToDto).ToList(),
        Images = product.Images.OrderBy(i => i.DisplayOrder).Select(ToDto).ToList(),
    };

    public static ProductVariantDto ToDto(ProductVariant variant) => new()
    {
        Id = variant.Id,
        Sku = variant.Sku,
        Barcode = variant.Barcode,
        BarcodeType = variant.BarcodeType,
        Price = variant.Price,
        CompareAtPrice = variant.CompareAtPrice,
        CostPrice = variant.CostPrice,
        TaxRate = variant.TaxRate,
        TaxType = variant.TaxType,
        Weight = variant.Weight,
        Length = variant.Length,
        Width = variant.Width,
        Height = variant.Height,
        IsActive = variant.IsActive,
        AttributeValueIds = variant.AttributeValueIds.ToList(),
    };

    public static ProductImageDto ToDto(ProductImage image) => new()
    {
        Id = image.Id,
        Url = image.Url,
        DisplayOrder = image.DisplayOrder,
    };
}
