using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.SalesOrders.Common;
using RetailSphere.Contracts.Sales;
using RetailSphere.Domain.Inventory;
using RetailSphere.Domain.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SalesOrders.CancelSalesOrder;

/// <summary>
/// Voids a completed sale and reverses its stock impact — the mirror image of
/// CreateSalesOrderCommandHandler's decrement. Restores each line's quantity back
/// to the branch it was sold from; does not model partial returns (see the class
/// remarks on SalesOrder.Cancel).
/// </summary>
public sealed class CancelSalesOrderCommandHandler(
    ISalesOrderRepository salesOrderRepository,
    IStockItemRepository stockItemRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    SalesOrderDtoAssembler salesOrderDtoAssembler)
    : IRequestHandler<CancelSalesOrderCommand, Result<SalesOrderDto>>
{
    public async Task<Result<SalesOrderDto>> Handle(CancelSalesOrderCommand request, CancellationToken cancellationToken)
    {
        var salesOrder = await salesOrderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (salesOrder is null)
            return Result.Failure<SalesOrderDto>(Error.NotFound("SalesOrder.NotFound", "Sales order not found."));

        var cancelResult = salesOrder.Cancel(request.Reason);
        if (cancelResult.IsFailure)
            return Result.Failure<SalesOrderDto>(cancelResult.Error);

        foreach (var line in salesOrder.Lines)
        {
            var stockItem = await stockItemRepository.GetByVariantAndBranchAsync(line.ProductVariantId, salesOrder.BranchId, cancellationToken);
            if (stockItem is null)
                continue; // Shouldn't happen — the sale itself created/updated this row — but don't block a cancellation on a missing ledger row.

            var adjustResult = stockItem.AdjustQuantity(line.Quantity, $"Cancelled sales order '{salesOrder.OrderNumber}': {request.Reason}", "SalesCancelled");
            if (adjustResult.IsFailure)
                return Result.Failure<SalesOrderDto>(adjustResult.Error);

            stockItemRepository.Update(stockItem);
        }

        salesOrderRepository.Update(salesOrder);
        auditLogService.Log("SalesOrder", salesOrder.Id.ToString(), "Cancelled", $"Cancelled sales order '{salesOrder.OrderNumber}': {request.Reason}");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await salesOrderDtoAssembler.ToDtoAsync(salesOrder, cancellationToken);
        return Result.Success(dto);
    }
}
