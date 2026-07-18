using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Inventory;

/// <summary>
/// A child entity of StockItem — one historical entry in a variant's stock ledger
/// at a branch. Immutable once created (there's no Update method); the running
/// QuantityOnHand lives on the parent StockItem, this is purely the "why did it
/// change" audit trail (who/what moved it, by how much, and why).
/// </summary>
public sealed class StockAdjustment : Entity<long>, IAuditableEntity
{
    /// <summary>Where a quantity change came from — a fixed, known set (same string-array pattern as ProductVariant.BarcodeTypes/TaxTypes and PurchaseOrder.Statuses).</summary>
    public static readonly IReadOnlyList<string> Sources = ["Initial", "Manual", "PurchaseOrderReceipt", "StockTransferOut", "StockTransferIn", "Sale", "SalesCancelled"];

    public long StockItemId { get; private set; }

    public decimal QuantityDelta { get; private set; }

    public string Reason { get; private set; } = default!;

    public string Source { get; private set; } = "Manual";

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }

    private StockAdjustment()
    {
    }

    internal static StockAdjustment Create(long stockItemId, decimal quantityDelta, string reason, string source) => new()
    {
        StockItemId = stockItemId,
        QuantityDelta = quantityDelta,
        Reason = reason.Trim(),
        Source = NormalizeSource(source),
    };

    private static string NormalizeSource(string? source) =>
        !string.IsNullOrWhiteSpace(source) && Sources.Contains(source) ? source : "Manual";
}
