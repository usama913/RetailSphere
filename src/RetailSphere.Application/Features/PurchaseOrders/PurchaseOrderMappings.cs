using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;

namespace RetailSphere.Application.Features.PurchaseOrders;

internal static class PurchaseOrderMappings
{
    public static PurchaseOrderDto ToDto(PurchaseOrder purchaseOrder, string? supplierName, string? branchName) => new()
    {
        Id = purchaseOrder.Id,
        PoNumber = purchaseOrder.PoNumber,
        SupplierId = purchaseOrder.SupplierId,
        SupplierName = supplierName,
        BranchId = purchaseOrder.BranchId,
        BranchName = branchName,
        Status = purchaseOrder.Status,
        OrderDate = purchaseOrder.OrderDate,
        ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
        Notes = purchaseOrder.Notes,
        TotalAmount = purchaseOrder.TotalAmount,
        Lines = purchaseOrder.Lines.Select(ToDto).ToList(),
    };

    public static PurchaseOrderLineDto ToDto(PurchaseOrderLine line) => new()
    {
        Id = line.Id,
        ProductId = line.ProductId,
        ProductVariantId = line.ProductVariantId,
        SkuSnapshot = line.SkuSnapshot,
        DescriptionSnapshot = line.DescriptionSnapshot,
        QuantityOrdered = line.QuantityOrdered,
        QuantityReceived = line.QuantityReceived,
        UnitCost = line.UnitCost,
        LineTotal = line.LineTotal,
        RemainingQuantity = line.RemainingQuantity,
        IsFullyReceived = line.IsFullyReceived,
    };
}
