using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Purchasing;

/// <summary>
/// A child entity of PurchaseOrder — one ordered SKU. SkuSnapshot/DescriptionSnapshot
/// are captured at order time (not looked up live from Catalog) so a PO's history
/// stays accurate even if the product/variant is later renamed or deleted — the same
/// "snapshot the validated value, don't keep re-resolving it" principle used for
/// ProductVariant.Sku/Price throughout this codebase.
/// </summary>
public sealed class PurchaseOrderLine : Entity<long>
{
    public long PurchaseOrderId { get; private set; }

    public long ProductId { get; private set; }

    public long ProductVariantId { get; private set; }

    public string SkuSnapshot { get; private set; } = default!;

    public string DescriptionSnapshot { get; private set; } = default!;

    public decimal QuantityOrdered { get; private set; }

    public decimal QuantityReceived { get; private set; }

    public decimal UnitCost { get; private set; }

    public decimal LineTotal => QuantityOrdered * UnitCost;

    public decimal RemainingQuantity => QuantityOrdered - QuantityReceived;

    public bool IsFullyReceived => QuantityReceived >= QuantityOrdered;

    private PurchaseOrderLine()
    {
    }

    internal static PurchaseOrderLine Create(
        long purchaseOrderId,
        long productId,
        long productVariantId,
        string skuSnapshot,
        string descriptionSnapshot,
        decimal quantityOrdered,
        decimal unitCost) => new()
        {
            PurchaseOrderId = purchaseOrderId,
            ProductId = productId,
            ProductVariantId = productVariantId,
            SkuSnapshot = skuSnapshot,
            DescriptionSnapshot = descriptionSnapshot,
            QuantityOrdered = quantityOrdered,
            QuantityReceived = 0,
            UnitCost = unitCost,
        };

    internal void UpdateQuantityAndCost(decimal quantityOrdered, decimal unitCost)
    {
        QuantityOrdered = quantityOrdered;
        UnitCost = unitCost;
    }

    internal void AddReceivedQuantity(decimal quantity) => QuantityReceived += quantity;
}
