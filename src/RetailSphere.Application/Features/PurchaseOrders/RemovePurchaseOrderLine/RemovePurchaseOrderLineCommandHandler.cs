using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.PurchaseOrders.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.RemovePurchaseOrderLine;

public sealed class RemovePurchaseOrderLineCommandHandler(
    IPurchaseOrderRepository purchaseOrderRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    PurchaseOrderDtoAssembler purchaseOrderDtoAssembler)
    : IRequestHandler<RemovePurchaseOrderLineCommand, Result<PurchaseOrderDto>>
{
    public async Task<Result<PurchaseOrderDto>> Handle(RemovePurchaseOrderLineCommand request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await purchaseOrderRepository.GetByIdAsync(request.PurchaseOrderId, cancellationToken);
        if (purchaseOrder is null)
            return Result.Failure<PurchaseOrderDto>(Error.NotFound("PurchaseOrder.NotFound", "Purchase order not found."));

        var removeResult = purchaseOrder.RemoveLine(request.LineId);
        if (removeResult.IsFailure)
            return Result.Failure<PurchaseOrderDto>(removeResult.Error);

        purchaseOrderRepository.Update(purchaseOrder);
        auditLogService.Log("PurchaseOrder", purchaseOrder.Id.ToString(), "LineRemoved", $"Removed a line from purchase order '{purchaseOrder.PoNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await purchaseOrderDtoAssembler.ToDtoAsync(purchaseOrder, cancellationToken);
        return Result.Success(dto);
    }
}
