using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.PurchaseOrders.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Catalog;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.AddPurchaseOrderLine;

/// <summary>
/// Resolves the ordered SKU from Catalog and snapshots its Sku/description onto the
/// line at order time (see the remarks on PurchaseOrderLine) — the request only
/// supplies the Ids, never the snapshot text, so it can't be spoofed by the client.
/// </summary>
public sealed class AddPurchaseOrderLineCommandHandler(
    IPurchaseOrderRepository purchaseOrderRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    PurchaseOrderDtoAssembler purchaseOrderDtoAssembler)
    : IRequestHandler<AddPurchaseOrderLineCommand, Result<PurchaseOrderDto>>
{
    public async Task<Result<PurchaseOrderDto>> Handle(AddPurchaseOrderLineCommand request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await purchaseOrderRepository.GetByIdAsync(request.PurchaseOrderId, cancellationToken);
        if (purchaseOrder is null)
            return Result.Failure<PurchaseOrderDto>(Error.NotFound("PurchaseOrder.NotFound", "Purchase order not found."));

        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result.Failure<PurchaseOrderDto>(Error.NotFound("Product.NotFound", "Product not found."));

        var variant = product.Variants.FirstOrDefault(v => v.Id == request.ProductVariantId);
        if (variant is null)
            return Result.Failure<PurchaseOrderDto>(Error.NotFound("Product.VariantNotFound", "Product variant not found."));

        var descriptionSnapshot = $"{product.Name} ({variant.Sku})";

        var addResult = purchaseOrder.AddLine(product.Id, variant.Id, variant.Sku, descriptionSnapshot, request.QuantityOrdered, request.UnitCost);
        if (addResult.IsFailure)
            return Result.Failure<PurchaseOrderDto>(addResult.Error);

        purchaseOrderRepository.Update(purchaseOrder);
        auditLogService.Log("PurchaseOrder", purchaseOrder.Id.ToString(), "LineAdded", $"Added line '{variant.Sku}' to purchase order '{purchaseOrder.PoNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await purchaseOrderDtoAssembler.ToDtoAsync(purchaseOrder, cancellationToken);
        return Result.Success(dto);
    }
}
