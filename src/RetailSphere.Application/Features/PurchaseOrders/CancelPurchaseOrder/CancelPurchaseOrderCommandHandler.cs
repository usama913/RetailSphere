using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.PurchaseOrders.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.CancelPurchaseOrder;

public sealed class CancelPurchaseOrderCommandHandler(
    IPurchaseOrderRepository purchaseOrderRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    PurchaseOrderDtoAssembler purchaseOrderDtoAssembler)
    : IRequestHandler<CancelPurchaseOrderCommand, Result<PurchaseOrderDto>>
{
    public async Task<Result<PurchaseOrderDto>> Handle(CancelPurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await purchaseOrderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (purchaseOrder is null)
            return Result.Failure<PurchaseOrderDto>(Error.NotFound("PurchaseOrder.NotFound", "Purchase order not found."));

        var cancelResult = purchaseOrder.Cancel();
        if (cancelResult.IsFailure)
            return Result.Failure<PurchaseOrderDto>(cancelResult.Error);

        purchaseOrderRepository.Update(purchaseOrder);
        auditLogService.Log("PurchaseOrder", purchaseOrder.Id.ToString(), "Cancelled", $"Cancelled purchase order '{purchaseOrder.PoNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await purchaseOrderDtoAssembler.ToDtoAsync(purchaseOrder, cancellationToken);
        return Result.Success(dto);
    }
}
