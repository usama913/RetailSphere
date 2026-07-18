using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Purchasing;

/// <summary>
/// Aggregate root: Purchase Order — a request to a Supplier for stock, scoped to
/// the Branch that will receive it. Owns its Lines; nothing outside this aggregate
/// mutates them directly (same rule as every other aggregate root in this codebase).
///
/// Status is a plain string (not a real C# enum) with a fixed allowed set, matching
/// the ProductVariant.BarcodeTypes/TaxTypes precedent from the Catalog module —
/// avoids introducing an unverified EF enum-conversion mapping with no local
/// compiler to check it against.
///
/// ReceiveLine only records QuantityReceived on the line itself — it does not touch
/// stock. ReceivePurchaseOrderLineCommandHandler is the actual Inventory integration
/// point: after calling ReceiveLine, it separately gets-or-creates the StockItem for
/// (line.ProductVariantId, BranchId) and adjusts it. Kept as two steps rather than
/// having this method reach into Inventory directly, since PurchaseOrder shouldn't
/// depend on another aggregate's repository.
/// </summary>
public sealed class PurchaseOrder : AggregateRoot<long>, IAuditableEntity, ISoftDeletable
{
    public static readonly IReadOnlyList<string> Statuses = ["Draft", "Submitted", "PartiallyReceived", "Received", "Cancelled"];

    private readonly List<PurchaseOrderLine> _lines = [];

    public string PoNumber { get; private set; } = default!;

    public long SupplierId { get; private set; }

    public long BranchId { get; set; }

    public string Status { get; private set; } = "Draft";

    public DateTime OrderDate { get; private set; }

    public DateTime? ExpectedDeliveryDate { get; private set; }

    public string? Notes { get; private set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public long? DeletedBy { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }

    public IReadOnlyCollection<PurchaseOrderLine> Lines => _lines.AsReadOnly();

    public decimal TotalAmount => _lines.Sum(l => l.LineTotal);

    private PurchaseOrder()
    {
    }

    public static Result<PurchaseOrder> Create(
        string poNumber,
        long supplierId,
        long branchId,
        DateTime orderDate,
        DateTime? expectedDeliveryDate,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(poNumber))
            return Result.Failure<PurchaseOrder>(Error.Validation("PurchaseOrder.NumberRequired", "PO number is required."));

        return Result.Success(new PurchaseOrder
        {
            PoNumber = poNumber.Trim(),
            SupplierId = supplierId,
            BranchId = branchId,
            Status = "Draft",
            OrderDate = orderDate,
            ExpectedDeliveryDate = expectedDeliveryDate,
            Notes = notes,
        });
    }

    public Result UpdateDetails(long supplierId, long branchId, DateTime orderDate, DateTime? expectedDeliveryDate, string? notes)
    {
        if (Status != "Draft")
            return Result.Failure(Error.Conflict("PurchaseOrder.NotEditable", "Only draft purchase orders can be edited."));

        SupplierId = supplierId;
        BranchId = branchId;
        OrderDate = orderDate;
        ExpectedDeliveryDate = expectedDeliveryDate;
        Notes = notes;
        return Result.Success();
    }

    public Result<PurchaseOrderLine> AddLine(
        long productId,
        long productVariantId,
        string skuSnapshot,
        string descriptionSnapshot,
        decimal quantityOrdered,
        decimal unitCost)
    {
        if (Status != "Draft")
            return Result.Failure<PurchaseOrderLine>(Error.Conflict("PurchaseOrder.NotEditable", "Lines can only be added to a draft purchase order."));

        if (quantityOrdered <= 0)
            return Result.Failure<PurchaseOrderLine>(Error.Validation("PurchaseOrder.InvalidQuantity", "Quantity ordered must be greater than zero."));

        if (unitCost < 0)
            return Result.Failure<PurchaseOrderLine>(Error.Validation("PurchaseOrder.InvalidUnitCost", "Unit cost cannot be negative."));

        var line = PurchaseOrderLine.Create(Id, productId, productVariantId, skuSnapshot, descriptionSnapshot, quantityOrdered, unitCost);
        _lines.Add(line);
        return Result.Success(line);
    }

    public Result UpdateLine(long lineId, decimal quantityOrdered, decimal unitCost)
    {
        if (Status != "Draft")
            return Result.Failure(Error.Conflict("PurchaseOrder.NotEditable", "Lines can only be edited on a draft purchase order."));

        var line = _lines.FirstOrDefault(l => l.Id == lineId);
        if (line is null)
            return Result.Failure(Error.NotFound("PurchaseOrder.LineNotFound", "Purchase order line not found."));

        if (quantityOrdered <= 0)
            return Result.Failure(Error.Validation("PurchaseOrder.InvalidQuantity", "Quantity ordered must be greater than zero."));

        if (unitCost < 0)
            return Result.Failure(Error.Validation("PurchaseOrder.InvalidUnitCost", "Unit cost cannot be negative."));

        line.UpdateQuantityAndCost(quantityOrdered, unitCost);
        return Result.Success();
    }

    public Result RemoveLine(long lineId)
    {
        if (Status != "Draft")
            return Result.Failure(Error.Conflict("PurchaseOrder.NotEditable", "Lines can only be removed from a draft purchase order."));

        var line = _lines.FirstOrDefault(l => l.Id == lineId);
        if (line is null)
            return Result.Failure(Error.NotFound("PurchaseOrder.LineNotFound", "Purchase order line not found."));

        _lines.Remove(line);
        return Result.Success();
    }

    public Result Submit()
    {
        if (Status != "Draft")
            return Result.Failure(Error.Conflict("PurchaseOrder.InvalidStatus", "Only draft purchase orders can be submitted."));

        if (_lines.Count == 0)
            return Result.Failure(Error.Validation("PurchaseOrder.NoLines", "Add at least one line before submitting."));

        Status = "Submitted";
        return Result.Success();
    }

    public Result ReceiveLine(long lineId, decimal quantity)
    {
        if (Status is not ("Submitted" or "PartiallyReceived"))
            return Result.Failure(Error.Conflict("PurchaseOrder.InvalidStatus", "Only submitted purchase orders can receive stock."));

        var line = _lines.FirstOrDefault(l => l.Id == lineId);
        if (line is null)
            return Result.Failure(Error.NotFound("PurchaseOrder.LineNotFound", "Purchase order line not found."));

        if (quantity <= 0)
            return Result.Failure(Error.Validation("PurchaseOrder.InvalidQuantity", "Received quantity must be greater than zero."));

        if (quantity > line.RemainingQuantity)
            return Result.Failure(Error.Validation("PurchaseOrder.OverReceipt", "Received quantity exceeds the remaining ordered quantity."));

        line.AddReceivedQuantity(quantity);

        Status = _lines.All(l => l.IsFullyReceived) ? "Received" : "PartiallyReceived";
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status is not ("Draft" or "Submitted" or "PartiallyReceived"))
            return Result.Failure(Error.Conflict("PurchaseOrder.InvalidStatus", "This purchase order can no longer be cancelled."));

        Status = "Cancelled";
        return Result.Success();
    }
}
