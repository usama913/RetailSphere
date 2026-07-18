using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Inventory;

/// <summary>
/// Aggregate root: how much of one ProductVariant is on hand at one Branch.
/// Exactly one StockItem exists per (ProductVariantId, BranchId) pair (enforced by
/// a unique index in Persistence) — created lazily the first time stock is ever
/// adjusted for that combination, rather than up front when a variant is created
/// (variants start with no stock; see the Products.razor variant generator).
///
/// ProductVariantId/BranchId are plain columns, not EF navigations — ProductVariant
/// is a child of the independent Product aggregate and Branch is its own aggregate,
/// same rule as every cross-aggregate reference elsewhere in this codebase.
///
/// Owns its Adjustments (the "why did this change" ledger); QuantityOnHand itself
/// is a running total stored directly on this aggregate rather than recomputed from
/// the ledger every read, the same "store the total, keep the log for audit only"
/// choice as PurchaseOrderLine.QuantityReceived vs its ledger of receipts.
/// </summary>
public sealed class StockItem : AggregateRoot<long>, IAuditableEntity
{
    private readonly List<StockAdjustment> _adjustments = [];

    public long ProductVariantId { get; private set; }

    public long BranchId { get; private set; }

    public decimal QuantityOnHand { get; private set; }

    public IReadOnlyCollection<StockAdjustment> Adjustments => _adjustments.AsReadOnly();

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }

    private StockItem()
    {
    }

    public static Result<StockItem> Create(long productVariantId, long branchId) => Result.Success(new StockItem
    {
        ProductVariantId = productVariantId,
        BranchId = branchId,
        QuantityOnHand = 0,
    });

    /// <summary>
    /// Applies a signed quantity change (positive to add stock, negative to remove
    /// it) and appends the audit-trail entry explaining why. Rejects any adjustment
    /// that would push QuantityOnHand below zero — this system doesn't model
    /// backorders/negative stock.
    /// </summary>
    public Result AdjustQuantity(decimal delta, string reason, string source)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(Error.Validation("StockItem.ReasonRequired", "A reason is required for every stock adjustment."));

        var newQuantity = QuantityOnHand + delta;
        if (newQuantity < 0)
            return Result.Failure(Error.Validation("StockItem.NegativeQuantity", "This adjustment would result in negative stock."));

        QuantityOnHand = newQuantity;
        _adjustments.Add(StockAdjustment.Create(Id, delta, reason, source));
        return Result.Success();
    }
}
