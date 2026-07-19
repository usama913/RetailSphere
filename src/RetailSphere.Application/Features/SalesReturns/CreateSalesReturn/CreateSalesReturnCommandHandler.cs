using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.SalesReturns.Common;
using RetailSphere.Common;
using RetailSphere.Contracts.Sales;
using RetailSphere.Domain.Inventory;
using RetailSphere.Domain.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SalesReturns.CreateSalesReturn;

/// <summary>
/// Processes a return against an existing SalesOrder: validates every requested
/// line against how much of that original line is still un-returned, builds the
/// whole SalesReturn (Create -> AddLine per item) with a month-scoped retry loop
/// for the return number (same pattern as CreateSalesOrderCommandHandler/
/// CreatePurchaseOrderCommandHandler), then — once the return itself is safely
/// persisted — marks the original lines' QuantityReturned and restocks each
/// returned item at the sale's branch, in a second SaveChangesAsync pass (same
/// "two-phase, not fully atomic across both phases" trade-off already accepted
/// for CreateSalesOrderCommandHandler's stock decrement).
/// </summary>
public sealed class CreateSalesReturnCommandHandler(
    ISalesReturnRepository salesReturnRepository,
    ISalesOrderRepository salesOrderRepository,
    IStockItemRepository stockItemRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    SalesReturnDtoAssembler salesReturnDtoAssembler)
    : IRequestHandler<CreateSalesReturnCommand, Result<SalesReturnDto>>
{
    private const int MaxReturnNumberAttempts = 5;

    public async Task<Result<SalesReturnDto>> Handle(CreateSalesReturnCommand request, CancellationToken cancellationToken)
    {
        var salesOrder = await salesOrderRepository.GetByIdAsync(request.SalesOrderId, cancellationToken);
        if (salesOrder is null)
            return Result.Failure<SalesReturnDto>(Error.NotFound("SalesOrder.NotFound", "The original sale was not found."));

        if (salesOrder.Status != "Completed")
            return Result.Failure<SalesReturnDto>(Error.Conflict("SalesReturn.OrderNotCompleted", "Only a completed sale can have items returned."));

        // Resolve every requested line up front (and check it against how much of the
        // original line is still un-returned) before touching anything, so a bad line
        // fails the whole return instead of a partial one.
        var resolvedLines = new List<(SalesOrderLine OriginalLine, decimal Quantity, decimal AllocatedDiscount)>();
        foreach (var requestedLine in request.Lines)
        {
            var originalLine = salesOrder.Lines.FirstOrDefault(l => l.Id == requestedLine.SalesOrderLineId);
            if (originalLine is null)
                return Result.Failure<SalesReturnDto>(Error.NotFound("SalesOrder.LineNotFound", $"Line {requestedLine.SalesOrderLineId} was not found on this sale."));

            var remaining = originalLine.Quantity - originalLine.QuantityReturned;
            if (requestedLine.Quantity > remaining)
                return Result.Failure<SalesReturnDto>(Error.Validation(
                    "SalesReturn.ExceedsRemaining",
                    $"Cannot return {requestedLine.Quantity} of '{originalLine.SkuSnapshot}' — only {remaining} remains un-returned."));

            var allocatedDiscount = originalLine.Quantity == 0 ? 0 : originalLine.DiscountAmount * (requestedLine.Quantity / originalLine.Quantity);
            resolvedLines.Add((originalLine, requestedLine.Quantity, allocatedDiscount));
        }

        SalesReturn salesReturn;
        var attempt = 1;
        while (true)
        {
            var returnNumber = await GenerateReturnNumberAsync(cancellationToken);

            var createResult = SalesReturn.Create(
                returnNumber, salesOrder.Id, salesOrder.BranchId, salesOrder.CustomerId, currentUserService.UserId, DateTime.UtcNow, request.Reason);
            if (createResult.IsFailure)
                return Result.Failure<SalesReturnDto>(createResult.Error);

            salesReturn = createResult.Value;

            foreach (var (originalLine, quantity, allocatedDiscount) in resolvedLines)
            {
                var addResult = salesReturn.AddLine(
                    originalLine.Id, originalLine.ProductId, originalLine.ProductVariantId, originalLine.SkuSnapshot, originalLine.DescriptionSnapshot,
                    quantity, originalLine.UnitPrice, originalLine.TaxRateSnapshot, originalLine.TaxTypeSnapshot, allocatedDiscount);
                if (addResult.IsFailure)
                    return Result.Failure<SalesReturnDto>(addResult.Error);
            }

            salesReturnRepository.Add(salesReturn);

            try
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                break;
            }
            catch (Exception ex) when (attempt < MaxReturnNumberAttempts && IsDuplicateReturnNumberViolation(ex))
            {
                salesReturnRepository.Remove(salesReturn);
                attempt++;
            }
        }

        // Mark the original lines returned, and restock each item at the sale's branch —
        // after the return itself is safely persisted (so it has a real Id for the
        // stock adjustment reason text).
        foreach (var (originalLine, quantity, _) in resolvedLines)
        {
            var increaseResult = originalLine.IncreaseReturnedQuantity(quantity);
            if (increaseResult.IsFailure)
                return Result.Failure<SalesReturnDto>(increaseResult.Error);

            var stockItem = await stockItemRepository.GetByVariantAndBranchAsync(originalLine.ProductVariantId, salesOrder.BranchId, cancellationToken);
            if (stockItem is null)
                return Result.Failure<SalesReturnDto>(Error.NotFound("StockItem.NotFound", $"Stock record for '{originalLine.SkuSnapshot}' not found."));

            var adjustResult = stockItem.AdjustQuantity(quantity, $"Returned via return '{salesReturn.ReturnNumber}'.", "Return");
            if (adjustResult.IsFailure)
                return Result.Failure<SalesReturnDto>(adjustResult.Error);

            stockItemRepository.Update(stockItem);
        }

        salesOrderRepository.Update(salesOrder);
        auditLogService.Log("SalesReturn", salesReturn.Id.ToString(), "Created", $"Processed return '{salesReturn.ReturnNumber}' for {salesReturn.RefundAmount:0.00} against sale '{salesOrder.OrderNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await salesReturnDtoAssembler.ToDtoAsync(salesReturn, cancellationToken);
        return Result.Success(dto);
    }

    /// <summary>Same duplicate-detection approach as CreateSalesOrderCommandHandler.IsDuplicateOrderNumberViolation — see its remarks.</summary>
    private static bool IsDuplicateReturnNumberViolation(Exception ex) =>
        ex.GetType().Name == "DbUpdateException"
        && (ex.InnerException?.Message.Contains("IX_SalesReturns_ReturnNumber", StringComparison.OrdinalIgnoreCase) == true
            || ex.InnerException?.Message.Contains("Duplicate entry", StringComparison.OrdinalIgnoreCase) == true);

    /// <summary>Month-scoped return number (e.g. "SR-202607-0001") — same pattern as CreateSalesOrderCommandHandler.GenerateOrderNumberAsync.</summary>
    private async Task<string> GenerateReturnNumberAsync(CancellationToken cancellationToken)
    {
        var prefix = $"SR-{DateTime.UtcNow:yyyyMM}-";
        var sequence = 1;
        while (true)
        {
            var candidate = $"{prefix}{sequence:0000}";
            if (!await salesReturnRepository.ReturnNumberExistsAsync(candidate, cancellationToken))
                return candidate;

            sequence++;
        }
    }
}
