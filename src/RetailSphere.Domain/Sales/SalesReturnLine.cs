using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Sales;

/// <summary>
/// A child entity of SalesReturn — one returned line item. Snapshots UnitPrice/
/// TaxRateSnapshot/TaxTypeSnapshot/DiscountAmount from the *original* SalesOrderLine
/// (proportional to how much of that line is being returned) so RefundAmount reflects
/// what the customer actually paid for those units, not today's catalog price.
/// </summary>
public sealed class SalesReturnLine : Entity<long>
{
    public long SalesReturnId { get; private set; }

    /// <summary>The original SalesOrderLine this return line is reversing — used to validate against over-returning and to trace a return back to its sale.</summary>
    public long SalesOrderLineId { get; private set; }

    public long ProductId { get; private set; }

    public long ProductVariantId { get; private set; }

    public string SkuSnapshot { get; private set; } = default!;

    public string DescriptionSnapshot { get; private set; } = default!;

    public decimal Quantity { get; private set; }

    /// <summary>Snapshot of the original line's UnitPrice.</summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>Snapshot of the original line's TaxRateSnapshot.</summary>
    public decimal TaxRateSnapshot { get; private set; }

    /// <summary>Snapshot of the original line's TaxTypeSnapshot.</summary>
    public string TaxTypeSnapshot { get; private set; } = "Exclusive";

    /// <summary>The portion of the original line's DiscountAmount attributable to these returned units (proportional to Quantity / original line quantity).</summary>
    public decimal DiscountAmount { get; private set; }

    /// <summary>Tax portion of this returned line, derived the same way as SalesOrderLine.TaxAmount.</summary>
    public decimal TaxAmount => TaxTypeSnapshot == "Inclusive"
        ? (UnitPrice * Quantity) - (UnitPrice * Quantity / (1 + TaxRateSnapshot / 100m))
        : UnitPrice * Quantity * TaxRateSnapshot / 100m;

    /// <summary>What this line refunds — same formula shape as SalesOrderLine.LineTotal.</summary>
    public decimal RefundAmount => TaxTypeSnapshot == "Inclusive"
        ? (UnitPrice * Quantity) - DiscountAmount
        : (UnitPrice * Quantity) + TaxAmount - DiscountAmount;

    private SalesReturnLine()
    {
    }

    internal static SalesReturnLine Create(
        long salesReturnId,
        long salesOrderLineId,
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
            SalesReturnId = salesReturnId,
            SalesOrderLineId = salesOrderLineId,
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
