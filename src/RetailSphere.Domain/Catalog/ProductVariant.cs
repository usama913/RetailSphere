using RetailSphere.Domain.Catalog.ValueObjects;
using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Catalog;

/// <summary>
/// The actual sellable unit — a SKU (§3: "Every sellable unit is a SKU (Product
/// Variant), never a bare Product; a Product is a non-transactable template.").
/// A child entity of Product; only ever created/mutated through the owning
/// Product aggregate, never persisted independently.
///
/// Sku/Barcode/Price are stored as plain string/decimal here rather than as the
/// Sku/Barcode/Money value objects directly — Money in particular has two
/// scalar parts (Amount + CurrencyCode), and mapping a multi-property value
/// object as a column on this table is more EF Core surface area than a single
/// Pakistan-only currency (the launch decision already baked into
/// Branch.CurrencyCode) currently needs. Callers still validate through
/// Sku.Create/Barcode.Create/Money.Create before reaching these methods —
/// only the already-validated `.Value`/`.Amount` gets stored.
///
/// Deliberately has no UnitOfMeasure of its own (removed — previously a free-text
/// string defaulting to "Each") — every variant of a Product sells in that
/// Product's Unit (Product.UnitId, master data), so a per-variant override was
/// both redundant and a source of inconsistent values ("Piece" vs "Pieces" vs "pcs").
/// </summary>
public sealed class ProductVariant : Entity<long>
{
    /// <summary>Barcode symbologies the label printer/scanner pipeline is expected to support.</summary>
    public static readonly IReadOnlyList<string> BarcodeTypes = ["C128", "C39", "EAN13", "EAN8", "UPCA", "UPCE", "QRCODE"];

    /// <summary>Whether the entered selling Price already includes TaxRate, or tax is added on top at sale time.</summary>
    public static readonly IReadOnlyList<string> TaxTypes = ["Exclusive", "Inclusive"];

    private readonly List<long> _attributeValueIds = [];

    public long ProductId { get; private set; }

    public string Sku { get; private set; } = default!;

    public string? Barcode { get; private set; }

    /// <summary>e.g. "C128" (Code 128), "EAN13" — the symbology Barcode should be rendered/scanned as.</summary>
    public string BarcodeType { get; private set; } = "C128";

    public decimal Price { get; private set; }

    public decimal? CompareAtPrice { get; private set; }

    /// <summary>Purchase/landed cost — kept separate from selling Price for margin reporting.</summary>
    public decimal? CostPrice { get; private set; }

    /// <summary>Tax percentage applied at sale time (e.g. 15.00 for 15%).</summary>
    public decimal TaxRate { get; private set; }

    /// <summary>"Exclusive" (tax added on top of Price) or "Inclusive" (Price already includes tax).</summary>
    public string TaxType { get; private set; } = "Exclusive";

    public decimal? Weight { get; private set; }

    public decimal? Length { get; private set; }

    public decimal? Width { get; private set; }

    public decimal? Height { get; private set; }

    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Optional low-stock threshold for this variant (same across all branches — a
    /// per-branch override would need a new branch-scoped entity, more surface area
    /// than Phase-1 stock visibility needs). Null means "no threshold set" — the
    /// variant never shows as low stock regardless of quantity.
    /// </summary>
    public decimal? ReorderPoint { get; private set; }

    /// <summary>Optional expiry/best-before date for this variant (e.g. perishable/dated stock) — feeds the "Products Near Expiry" dashboard widget. Null means the variant doesn't expire / isn't tracked for expiry.</summary>
    public DateTime? ExpiryDate { get; private set; }

    /// <summary>The AttributeValues (e.g. Size=42, Color=Red) that identify this specific variant.</summary>
    public IReadOnlyCollection<long> AttributeValueIds => _attributeValueIds.AsReadOnly();

    private ProductVariant()
    {
    }

    internal static ProductVariant Create(
        long productId,
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
        var variant = new ProductVariant
        {
            ProductId = productId,
            Sku = sku.Value,
            Price = price.Amount,
            Barcode = barcode?.Value,
            BarcodeType = NormalizeBarcodeType(barcodeType),
            CompareAtPrice = compareAtPrice?.Amount,
            CostPrice = costPrice?.Amount,
            TaxRate = taxRate,
            TaxType = NormalizeTaxType(taxType),
            Weight = weight,
            Length = length,
            Width = width,
            Height = height,
            ReorderPoint = reorderPoint,
            ExpiryDate = expiryDate,
            IsActive = true,
        };

        variant._attributeValueIds.AddRange(attributeValueIds.Distinct());
        return variant;
    }

    internal void UpdatePricing(Money price, Money? compareAtPrice, Money? costPrice, decimal taxRate, string? taxType)
    {
        Price = price.Amount;
        CompareAtPrice = compareAtPrice?.Amount;
        CostPrice = costPrice?.Amount;
        TaxRate = taxRate;
        TaxType = NormalizeTaxType(taxType);
    }

    internal void UpdatePhysicalAttributes(decimal? weight, decimal? length, decimal? width, decimal? height)
    {
        Weight = weight;
        Length = length;
        Width = width;
        Height = height;
    }

    internal void UpdateBarcode(Barcode? barcode, string? barcodeType)
    {
        Barcode = barcode?.Value;
        BarcodeType = NormalizeBarcodeType(barcodeType);
    }

    internal void UpdateReorderPoint(decimal? reorderPoint)
    {
        ReorderPoint = reorderPoint;
    }

    internal void UpdateExpiryDate(DateTime? expiryDate)
    {
        ExpiryDate = expiryDate;
    }

    private static string NormalizeBarcodeType(string? barcodeType) =>
        !string.IsNullOrWhiteSpace(barcodeType) && BarcodeTypes.Contains(barcodeType) ? barcodeType : "C128";

    private static string NormalizeTaxType(string? taxType) =>
        !string.IsNullOrWhiteSpace(taxType) && TaxTypes.Contains(taxType) ? taxType : "Exclusive";

    internal void UpdateAttributeSelections(IEnumerable<long> attributeValueIds)
    {
        _attributeValueIds.Clear();
        _attributeValueIds.AddRange(attributeValueIds.Distinct());
    }

    internal void Activate() => IsActive = true;

    internal void Deactivate() => IsActive = false;
}
