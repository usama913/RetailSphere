using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Sales;

/// <summary>
/// Aggregate root: Sales Return — a refund/exchange against a specific original
/// SalesOrder. Like SalesOrder itself, a return is built up and finalized in a
/// single action (CreateSalesReturnCommandHandler: Create, then AddLine per
/// returned item) rather than a multi-step Draft/Submit workflow — there's no
/// in-between state a return sits in. RefundAmount is computed from the lines
/// (mirrors SalesOrder.TotalAmount), never stored.
///
/// SalesOrderId/BranchId/CustomerId/ProcessedByUserId are plain columns, not EF
/// navigations — SalesOrder/Branch/Customer/User are independent aggregates,
/// the same rule as every other cross-aggregate reference in this codebase.
/// </summary>
public sealed class SalesReturn : AggregateRoot<long>, IAuditableEntity, ISoftDeletable
{
    private readonly List<SalesReturnLine> _lines = [];

    public string ReturnNumber { get; private set; } = default!;

    public long SalesOrderId { get; private set; }

    public long BranchId { get; private set; }

    public long? CustomerId { get; private set; }

    public long? ProcessedByUserId { get; private set; }

    public DateTime ReturnDate { get; private set; }

    public string? Reason { get; private set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public long? DeletedBy { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }

    public IReadOnlyCollection<SalesReturnLine> Lines => _lines.AsReadOnly();

    public decimal RefundAmount => _lines.Sum(l => l.RefundAmount);

    private SalesReturn()
    {
    }

    public static Result<SalesReturn> Create(
        string returnNumber,
        long salesOrderId,
        long branchId,
        long? customerId,
        long? processedByUserId,
        DateTime returnDate,
        string? reason)
    {
        if (string.IsNullOrWhiteSpace(returnNumber))
            return Result.Failure<SalesReturn>(Error.Validation("SalesReturn.NumberRequired", "Return number is required."));

        return Result.Success(new SalesReturn
        {
            ReturnNumber = returnNumber.Trim(),
            SalesOrderId = salesOrderId,
            BranchId = branchId,
            CustomerId = customerId,
            ProcessedByUserId = processedByUserId,
            ReturnDate = returnDate,
            Reason = reason,
        });
    }

    public Result<SalesReturnLine> AddLine(
        long salesOrderLineId,
        long productId,
        long productVariantId,
        string skuSnapshot,
        string descriptionSnapshot,
        decimal quantity,
        decimal unitPrice,
        decimal taxRateSnapshot,
        string? taxTypeSnapshot,
        decimal discountAmount)
    {
        if (quantity <= 0)
            return Result.Failure<SalesReturnLine>(Error.Validation("SalesReturn.InvalidQuantity", "Return quantity must be greater than zero."));

        var line = SalesReturnLine.Create(Id, salesOrderLineId, productId, productVariantId, skuSnapshot, descriptionSnapshot, quantity, unitPrice, taxRateSnapshot, taxTypeSnapshot, discountAmount);
        _lines.Add(line);
        return Result.Success(line);
    }
}
