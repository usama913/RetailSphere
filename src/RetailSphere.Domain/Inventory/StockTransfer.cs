using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Inventory;

/// <summary>
/// Aggregate root: Stock Transfer — moves a quantity of a ProductVariant from one
/// Branch to another. Owns its Lines, same rule as every other aggregate root here.
///
/// Mirrors PurchaseOrder's workflow deliberately: FromBranchId plays the role
/// Supplier plays for a PurchaseOrder (the source), ToBranchId plays the role the
/// PurchaseOrder's own Branch plays (the receiver). Just like PurchaseOrder, Submit
/// only changes Status — it does not touch stock. ReceiveStockTransferLineCommandHandler
/// is the actual Inventory integration point: after calling ReceiveLine, it separately
/// decrements the StockItem at FromBranchId and increments the StockItem at
/// ToBranchId for the line's ProductVariantId, by the received quantity. Kept as two
/// steps rather than reaching into Inventory directly from this aggregate, for the
/// same reason PurchaseOrder doesn't touch Inventory directly either.
///
/// Because no stock moves until a line is actually received, Cancel never needs to
/// unwind a partial shipment — the same simplifying property PurchaseOrder relies on.
/// </summary>
public sealed class StockTransfer : AggregateRoot<long>, IAuditableEntity, ISoftDeletable
{
    public static readonly IReadOnlyList<string> Statuses = ["Draft", "Submitted", "PartiallyReceived", "Received", "Cancelled"];

    private readonly List<StockTransferLine> _lines = [];

    public string TransferNumber { get; private set; } = default!;

    public long FromBranchId { get; private set; }

    public long ToBranchId { get; private set; }

    public string Status { get; private set; } = "Draft";

    public DateTime TransferDate { get; private set; }

    public string? Notes { get; private set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public long? DeletedBy { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }

    public IReadOnlyCollection<StockTransferLine> Lines => _lines.AsReadOnly();

    public decimal TotalQuantityRequested => _lines.Sum(l => l.QuantityRequested);

    private StockTransfer()
    {
    }

    public static Result<StockTransfer> Create(
        string transferNumber,
        long fromBranchId,
        long toBranchId,
        DateTime transferDate,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(transferNumber))
            return Result.Failure<StockTransfer>(Error.Validation("StockTransfer.NumberRequired", "Transfer number is required."));

        if (fromBranchId == toBranchId)
            return Result.Failure<StockTransfer>(Error.Validation("StockTransfer.SameBranch", "Source and destination branches must be different."));

        return Result.Success(new StockTransfer
        {
            TransferNumber = transferNumber.Trim(),
            FromBranchId = fromBranchId,
            ToBranchId = toBranchId,
            Status = "Draft",
            TransferDate = transferDate,
            Notes = notes,
        });
    }

    public Result UpdateDetails(long fromBranchId, long toBranchId, DateTime transferDate, string? notes)
    {
        if (Status != "Draft")
            return Result.Failure(Error.Conflict("StockTransfer.NotEditable", "Only draft transfers can be edited."));

        if (fromBranchId == toBranchId)
            return Result.Failure(Error.Validation("StockTransfer.SameBranch", "Source and destination branches must be different."));

        FromBranchId = fromBranchId;
        ToBranchId = toBranchId;
        TransferDate = transferDate;
        Notes = notes;
        return Result.Success();
    }

    public Result<StockTransferLine> AddLine(
        long productId,
        long productVariantId,
        string skuSnapshot,
        string descriptionSnapshot,
        decimal quantityRequested)
    {
        if (Status != "Draft")
            return Result.Failure<StockTransferLine>(Error.Conflict("StockTransfer.NotEditable", "Lines can only be added to a draft transfer."));

        if (quantityRequested <= 0)
            return Result.Failure<StockTransferLine>(Error.Validation("StockTransfer.InvalidQuantity", "Quantity requested must be greater than zero."));

        var line = StockTransferLine.Create(Id, productId, productVariantId, skuSnapshot, descriptionSnapshot, quantityRequested);
        _lines.Add(line);
        return Result.Success(line);
    }

    public Result UpdateLine(long lineId, decimal quantityRequested)
    {
        if (Status != "Draft")
            return Result.Failure(Error.Conflict("StockTransfer.NotEditable", "Lines can only be edited on a draft transfer."));

        var line = _lines.FirstOrDefault(l => l.Id == lineId);
        if (line is null)
            return Result.Failure(Error.NotFound("StockTransfer.LineNotFound", "Transfer line not found."));

        if (quantityRequested <= 0)
            return Result.Failure(Error.Validation("StockTransfer.InvalidQuantity", "Quantity requested must be greater than zero."));

        line.UpdateQuantity(quantityRequested);
        return Result.Success();
    }

    public Result RemoveLine(long lineId)
    {
        if (Status != "Draft")
            return Result.Failure(Error.Conflict("StockTransfer.NotEditable", "Lines can only be removed from a draft transfer."));

        var line = _lines.FirstOrDefault(l => l.Id == lineId);
        if (line is null)
            return Result.Failure(Error.NotFound("StockTransfer.LineNotFound", "Transfer line not found."));

        _lines.Remove(line);
        return Result.Success();
    }

    public Result Submit()
    {
        if (Status != "Draft")
            return Result.Failure(Error.Conflict("StockTransfer.InvalidStatus", "Only draft transfers can be submitted."));

        if (_lines.Count == 0)
            return Result.Failure(Error.Validation("StockTransfer.NoLines", "Add at least one line before submitting."));

        Status = "Submitted";
        return Result.Success();
    }

    public Result ReceiveLine(long lineId, decimal quantity)
    {
        if (Status is not ("Submitted" or "PartiallyReceived"))
            return Result.Failure(Error.Conflict("StockTransfer.InvalidStatus", "Only submitted transfers can receive stock."));

        var line = _lines.FirstOrDefault(l => l.Id == lineId);
        if (line is null)
            return Result.Failure(Error.NotFound("StockTransfer.LineNotFound", "Transfer line not found."));

        if (quantity <= 0)
            return Result.Failure(Error.Validation("StockTransfer.InvalidQuantity", "Received quantity must be greater than zero."));

        if (quantity > line.RemainingQuantity)
            return Result.Failure(Error.Validation("StockTransfer.OverReceipt", "Received quantity exceeds the remaining requested quantity."));

        line.AddReceivedQuantity(quantity);

        Status = _lines.All(l => l.IsFullyReceived) ? "Received" : "PartiallyReceived";
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status is not ("Draft" or "Submitted" or "PartiallyReceived"))
            return Result.Failure(Error.Conflict("StockTransfer.InvalidStatus", "This transfer can no longer be cancelled."));

        Status = "Cancelled";
        return Result.Success();
    }
}
