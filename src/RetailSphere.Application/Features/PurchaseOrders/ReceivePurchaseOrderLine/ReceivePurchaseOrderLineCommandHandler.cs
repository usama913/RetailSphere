using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.PurchaseOrders.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.ReceivePurchaseOrderLine;

/// <summary>
/// Records received quantity against a line. This does NOT yet touch branch stock
/// levels — see the remarks on PurchaseOrder.ReceiveLine for why that's a
/// deliberate, explicitly-flagged gap until the Inventory module exists.
/// </summary>
public sealed class ReceivePurchaseOrderLineCommandHandler(
    IPurchaseOrderRepository purchaseOrderRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    PurchaseOrderDtoAssembler purchaseOrderDtoAssembler)
    : IRequestHandler<ReceivePurchaseOrderLineCommand, Result<PurchaseOrderDto>>
{
    public async Task<Result<PurchaseOrderDto>> Handle(ReceivePurchaseOrderLineCommand request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await purchaseOrderRepository.GetByIdAsync(request.PurchaseOrderId, cancellationToken);
        if (purchaseOrder is null)
            return Result.Failure<PurchaseOrderDto>(Error.NotFound("PurchaseOrder.NotFound", "Purchase order not found."));

        var receiveResult = purchaseOrder.ReceiveLine(request.LineId, request.Quantity);
        if (receiveResult.IsFailure)
            return Result.Failure<PurchaseOrderDto>(receiveResult.Error);

        purchaseOrderRepository.Update(purchaseOrder);
        auditLogService.Log("PurchaseOrder", purchaseOrder.Id.ToString(), "LineReceived", $"Received {request.Quantity} unit(s) on purchase order '{purchaseOrder.PoNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await purchaseOrderDtoAssembler.ToDtoAsync(purchaseOrder, cancellationToken);
        return Result.Success(dto);
    }
}
