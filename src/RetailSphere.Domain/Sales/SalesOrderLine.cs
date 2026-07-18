using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Sales;

/// <summary>
/// A child entity of SalesOrder — one sold SKU. UnitPrice/TaxRateSnapshot/TaxTypeSnapshot
/// are captured at sale time (not looked up live from Catalog) so a completed sale's
/// receipt/history stays accurate even if the product's price or tax rate changes
/// later — same "snapshot the validated value" principle as PurchaseOrderLine.
/// </summary>
public sealed class SalesOrderLine : Entity<long>
{
    public long SalesOrderId { get; private set; }

    public long ProductId { get; private set; }

    public long ProductVariantId { get; private set; }

    public string SkuSnapshot { get; private set; } = default!;

    public string DescriptionSnapshot { get; private set; } = default!;

    public decimal Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal TaxRateSnapshot { get; private set; }

    /// <summary>"Exclusive" (tax added on top of UnitPrice) or "Inclusive" (UnitPrice already includes tax) — snapshot of ProductVariant.TaxType at sale time.</summary>
    public string TaxTypeSnapshot { get; private set; } = "Exclusive";

    public decimal DiscountAmount { get; private set; }

    /// <summary>Tax portion of this line, derived from the snapshot values — not stored.</summary>
    public decimal TaxAmount => TaxTypeSnapshot == "Inclusive"
        ? (UnitPrice * Quantity) - (UnitPrice * Quantity / (1 + TaxRateSnapshot / 100m))
        : UnitPrice * Quantity * TaxRateSnapshot / 100m;

    /// <summary>What this line contributes to the order total, after its own discount.</summary>
    public decimal LineTotal => TaxTypeSnapshot == "Inclusive"
        ? (UnitPrice * Quantity) - DiscountAmount
        : (UnitPrice * Quantity) + TaxAmount - DiscountAmount;

    private SalesOrderLine()
    {
    }

    internal static SalesOrderLine Create(
        long salesOrderId,
        long productId,
        long productVariantId,
        string skuSnapshot,
        string descriptionSnapshot,
        decimal quantity,
        decimal unitPrice,
        decimal taxRateSnapshot,
        string? taxTypeSnapshot,
        decimal discountAmount) => new()
        {
            SalesOrderId = salesOrderId,
            ProductId = productId,
            ProductVariantId = productVariantId,
            SkuSnapshot = skuSnapshot,
            DescriptionSnapshot = descriptionSnapshot,
            Quantity = quantity,
            UnitPrice = unitPrice,
            TaxRateSnapshot = taxRateSnapshot,
            TaxTypeSnapshot = taxTypeSnapshot is "Inclusive" or "Exclusive" ? taxTypeSnapshot : "Exclusive",
            DiscountAmount = discountAmount,
        };
}
