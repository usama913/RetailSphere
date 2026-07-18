namespace RetailSphere.Contracts.Catalog;

public sealed class ProductVariantDto
{
    public required long Id { get; init; }

    public required string Sku { get; init; }

    public string? Barcode { get; init; }

    public required string BarcodeType { get; init; }

    public required decimal Price { get; init; }

    public decimal? CompareAtPrice { get; init; }

    public decimal? CostPrice { get; init; }

    public required decimal TaxRate { get; init; }

    public required string TaxType { get; init; }

    public decimal? Weight { get; init; }

    public decimal? Length { get; init; }

    public decimal? Width { get; init; }

    public decimal? Height { get; init; }

    public required bool IsActive { get; init; }

    public required IReadOnlyList<long> AttributeValueIds { get; init; }
}

public sealed class ProductImageDto
{
    public required long Id { get; init; }

    public required string Url { get; init; }

    public required int DisplayOrder { get; init; }
}

public sealed class ProductDto
{
    public required long Id { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public long? CategoryId { get; init; }

    public string? CategoryName { get; init; }

    public long? BrandId { get; init; }

    public string? BrandName { get; init; }

    public long? UnitId { get; init; }

    public string? UnitName { get; init; }

    public required bool ManageStock { get; init; }

    public required bool NotForSelling { get; init; }

    public required bool IsActive { get; init; }

    public required IReadOnlyList<ProductVariantDto> Variants { get; init; }

    public required IReadOnlyList<ProductImageDto> Images { get; init; }
}

public sealed class CreateProductRequest
{
    public required string Name { get; init; }

    public string? Description { get; init; }

    public long? CategoryId { get; init; }

    public long? BrandId { get; init; }

    public long? UnitId { get; init; }

    public bool ManageStock { get; init; } = true;

    public bool NotForSelling { get; init; }
}

public sealed class UpdateProductRequest
{
    public required string Name { get; init; }

    public string? Description { get; init; }

    public long? CategoryId { get; init; }

    public long? BrandId { get; init; }

    public long? UnitId { get; init; }

    public bool ManageStock { get; init; } = true;

    public bool NotForSelling { get; init; }
}

public sealed class AddVariantRequest
{
    /// <summary>Optional — auto-generated (e.g. "P1042-V01") if left blank.</summary>
    public string? Sku { get; init; }

    public string? Barcode { get; init; }

    public string? BarcodeType { get; init; }

    public required decimal Price { get; init; }

    public decimal? CompareAtPrice { get; init; }

    public decimal? CostPrice { get; init; }

    public decimal TaxRate { get; init; }

    public string? TaxType { get; init; }

    public decimal? Weight { get; init; }

    public decimal? Length { get; init; }

    public decimal? Width { get; init; }

    public decimal? Height { get; init; }

    public IReadOnlyList<long> AttributeValueIds { get; init; } = [];
}

public sealed class UpdateVariantRequest
{
    public string? Barcode { get; init; }

    public string? BarcodeType { get; init; }

    public required decimal Price { get; init; }

    public decimal? CompareAtPrice { get; init; }

    public decimal? CostPrice { get; init; }

    public decimal TaxRate { get; init; }

    public string? TaxType { get; init; }

    public decimal? Weight { get; init; }

    public decimal? Length { get; init; }

    public decimal? Width { get; init; }

    public decimal? Height { get; init; }

    public IReadOnlyList<long> AttributeValueIds { get; init; } = [];
}

public sealed class AddProductImageRequest
{
    public required string Url { get; init; }
}
