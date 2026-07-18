using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.PurchaseOrders.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Organization;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.UpdatePurchaseOrder;

public sealed class UpdatePurchaseOrderCommandHandler(
    IPurchaseOrderRepository purchaseOrderRepository,
    ISupplierRepository supplierRepository,
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    PurchaseOrderDtoAssembler purchaseOrderDtoAssembler)
    : IRequestHandler<UpdatePurchaseOrderCommand, Result<PurchaseOrderDto>>
{
    public async Task<Result<PurchaseOrderDto>> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await purchaseOrderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (purchaseOrder is null)
            return Result.Failure<PurchaseOrderDto>(Error.NotFound("PurchaseOrder.NotFound", "Purchase order not found."));

        var supplier = await supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier is null)
            return Result.Failure<PurchaseOrderDto>(Error.NotFound("Supplier.NotFound", "Supplier not found."));

        var branch = await branchRepository.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null)
            return Result.Failure<PurchaseOrderDto>(Error.NotFound("Branch.NotFound", "Branch not found."));

        var updateResult = purchaseOrder.UpdateDetails(request.SupplierId, request.BranchId, request.OrderDate, request.ExpectedDeliveryDate, request.Notes);
        if (updateResult.IsFailure)
            return Result.Failure<PurchaseOrderDto>(updateResult.Error);

        purchaseOrderRepository.Update(purchaseOrder);
        auditLogService.Log("PurchaseOrder", purchaseOrder.Id.ToString(), "Updated", $"Updated purchase order '{purchaseOrder.PoNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await purchaseOrderDtoAssembler.ToDtoAsync(purchaseOrder, cancellationToken);
        return Result.Success(dto);
    }
}
