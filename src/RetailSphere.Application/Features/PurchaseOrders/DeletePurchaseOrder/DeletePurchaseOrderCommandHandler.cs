using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.DeletePurchaseOrder;

public sealed class DeletePurchaseOrderCommandHandler(
    IPurchaseOrderRepository purchaseOrderRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<DeletePurchaseOrderCommand, Result>
{
    public async Task<Result> Handle(DeletePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await purchaseOrderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (purchaseOrder is null)
            return Result.Failure(Error.NotFound("PurchaseOrder.NotFound", "Purchase order not found."));

        if (purchaseOrder.Status != "Draft")
            return Result.Failure(Error.Conflict("PurchaseOrder.NotDeletable", "Only draft purchase orders can be deleted — cancel it instead."));

        purchaseOrderRepository.Remove(purchaseOrder);
        auditLogService.Log("PurchaseOrder", purchaseOrder.Id.ToString(), "Deleted", $"Deleted purchase order '{purchaseOrder.PoNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
