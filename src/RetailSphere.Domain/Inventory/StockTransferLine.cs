using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Inventory;

/// <summary>
/// A child entity of StockTransfer — one transferred SKU. SkuSnapshot/DescriptionSnapshot
/// are captured at request time, mirroring PurchaseOrderLine's snapshot rationale, so a
/// transfer's history stays accurate even if the product/variant is later renamed.
/// </summary>
public sealed class StockTransferLine : Entity<long>
{
    public long StockTransferId { get; private set; }

    public long ProductId { get; private set; }

    public long ProductVariantId { get; private set; }

    public string SkuSnapshot { get; private set; } = default!;

    public string DescriptionSnapshot { get; private set; } = default!;

    public decimal QuantityRequested { get; private set; }

    public decimal QuantityReceived { get; private set; }

    public decimal RemainingQuantity => QuantityRequested - QuantityReceived;

    public bool IsFullyReceived => QuantityReceived >= QuantityRequested;

    private StockTransferLine()
    {
    }

    internal static StockTransferLine Create(
        long stockTransferId,
        long productId,
        long productVariantId,
        string skuSnapshot,
        string descriptionSnapshot,
        decimal quantityRequested) => new()
        {
            StockTransferId = stockTransferId,
            ProductId = productId,
            ProductVariantId = productVariantId,
            SkuSnapshot = skuSnapshot,
            DescriptionSnapshot = descriptionSnapshot,
            QuantityRequested = quantityRequested,
            QuantityReceived = 0,
        };

    internal void UpdateQuantity(decimal quantityRequested) => QuantityRequested = quantityRequested;

    internal void AddReceivedQuantity(decimal quantity) => QuantityReceived += quantity;
}
