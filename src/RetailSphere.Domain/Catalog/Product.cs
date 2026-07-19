using RetailSphere.Domain.Catalog.ValueObjects;
using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Catalog;

/// <summary>
/// Aggregate root: Product — a non-transactable template that expands into
/// many sellable SKUs (§3). Owns its Variants and product-level Images; nothing
/// outside this aggregate mutates them directly (repositories load/persist the
/// whole aggregate, same rule as every other aggregate root in this codebase).
/// CategoryId/BrandId are plain columns, not EF navigations — Category and
/// Brand are independent aggregates in their own right.
/// </summary>
public sealed class Product : AggregateRoot<long>, IAuditableEntity, ISoftDeletable
{
    private readonly List<ProductVariant> _variants = [];
    private readonly List<ProductImage> _images = [];

    public string Name { get; private set; } = default!;

    public string? Description { get; private set; }

    public long? CategoryId { get; private set; }

    public long? BrandId { get; private set; }

    /// <summary>Plain column, no EF navigation — Unit is an independent aggregate (see Category/Brand remarks).</summary>
    public long? UnitId { get; private set; }

    /// <summary>Whether inventory/stock quantities are tracked for this product.</summary>
    public bool ManageStock { get; private set; } = true;

    /// <summary>Marks a product as internal-only (e.g. raw material) — excluded from POS/sale screens.</summary>
    public bool NotForSelling { get; private set; }

    public bool IsActive { get; private set; } = true;

    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();

    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public long? DeletedBy { get; set; }

    private Product()
    {
    }

    public static Result<Product> Create(
        string name,
        string? description,
        long? categoryId,
        long? brandId,
        long? unitId,
        bool manageStock,
        bool notForSelling)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Product>(Error.Validation("Product.NameRequired", "Product name is required."));

        return Result.Success(new Product
        {
            Name = name.Trim(),
            Description = description,
            CategoryId = categoryId,
            BrandId = brandId,
            UnitId = unitId,
            ManageStock = manageStock,
            NotForSelling = notForSelling,
            IsActive = true,
        });
    }

    public Result UpdateDetails(
        string name,
        string? description,
        long? categoryId,
        long? brandId,
        long? unitId,
        bool manageStock,
        bool notForSelling)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation("Product.NameRequired", "Product name is required."));

        Name = name.Trim();
        Description = description;
        CategoryId = categoryId;
        BrandId = brandId;
        UnitId = unitId;
        ManageStock = manageStock;
        NotForSelling = notForSelling;
        return Result.Success();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public Result<ProductVariant> AddVariant(
        Sku sku,
        Money price,
        Barcode? barcode,
        string? barcodeType,
        Money? compareAtPrice,
        Money? costPrice,
        decimal taxRate,
        string? taxType,
        decimal? weight,
        decimal? length,
        decimal? width,
        decimal? height,
        decimal? reorderPoint,
        IEnumerable<long> attributeValueIds,
        DateTime? expiryDate = null)
    {
        if (_variants.Any(v => v.Sku == sku.Value))
            return Result.Failure<ProductVariant>(Error.Conflict("Product.DuplicateSku", "A variant with this SKU already exists on this product."));

        var variant = ProductVariant.Create(
            Id, sku, price, barcode, barcodeType, compareAtPrice, costPrice, taxRate, taxType, weight, length, width, height, reorderPoint, attributeValueIds, expiryDate);
        _variants.Add(variant);
        return Result.Success(variant);
    }

    public Result UpdateVariant(
        long variantId,
        Money price,
        Money? compareAtPrice,
        Money? costPrice,
        decimal taxRate,
        string? taxType,
        Barcode? barcode,
        string? barcodeType,
        decimal? weight,
        decimal? length,
        decimal? width,
        decimal? height,
        decimal? reorderPoint,
        IEnumerable<long> attributeValueIds,
        DateTime? expiryDate = null)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant is null)
            return Result.Failure(Error.NotFound("Product.VariantNotFound", "Variant not found."));

        variant.UpdatePricing(price, compareAtPrice, costPrice, taxRate, taxType);
        variant.UpdateBarcode(barcode, barcodeType);
        variant.UpdatePhysicalAttributes(weight, length, width, height);
        variant.UpdateReorderPoint(reorderPoint);
        variant.UpdateExpiryDate(expiryDate);
        variant.UpdateAttributeSelections(attributeValueIds);
        return Result.Success();
    }

    public Result RemoveVariant(long variantId)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant is null)
            return Result.Failure(Error.NotFound("Product.VariantNotFound", "Variant not found."));

        _variants.Remove(variant);
        return Result.Success();
    }

    public Result SetVariantActive(long variantId, bool isActive)
    {
        var variant = _variants.FirstOrDefault(v => v.Id == variantId);
        if (variant is null)
            return Result.Failure(Error.NotFound("Product.VariantNotFound", "Variant not found."));

        if (isActive)
            variant.Activate();
        else
            variant.Deactivate();

        return Result.Success();
    }

    public Result<ProductImage> AddImage(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return Result.Failure<ProductImage>(Error.Validation("Product.ImageUrlRequired", "Image URL is required."));

        var image = ProductImage.Create(Id, url, _images.Count);
        _images.Add(image);
        return Result.Success(image);
    }

    public Result RemoveImage(long imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image is null)
            return Result.Failure(Error.NotFound("Product.ImageNotFound", "Image not found."));

        _images.Remove(image);
        return Result.Success();
    }
}
