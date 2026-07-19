using RetailSphere.Contracts.Sales;
using RetailSphere.Domain.Sales;

namespace RetailSphere.Application.Features.SalesOrders;

internal static class SalesOrderMappings
{
    public static SalesOrderDto ToDto(SalesOrder salesOrder, string? branchName, string? customerName, string? cashierName) => new()
    {
        Id = salesOrder.Id,
        OrderNumber = salesOrder.OrderNumber,
        BranchId = salesOrder.BranchId,
        BranchName = branchName,
        CustomerId = salesOrder.CustomerId,
        CustomerName = customerName,
        CashierUserId = salesOrder.CashierUserId,
        CashierName = cashierName,
        Status = salesOrder.Status,
        OrderDate = salesOrder.OrderDate,
        PaymentMethod = salesOrder.PaymentMethod,
        SubtotalAmount = salesOrder.SubtotalAmount,
        TaxAmount = salesOrder.TaxAmount,
        OrderDiscountAmount = salesOrder.OrderDiscountAmount,
        TotalAmount = salesOrder.TotalAmount,
        AmountPaid = salesOrder.AmountPaid,
        ChangeDue = salesOrder.ChangeDue,
        Notes = salesOrder.Notes,
        CancellationReason = salesOrder.CancellationReason,
        Lines = salesOrder.Lines.Select(ToDto).ToList(),
    };

    public static SalesOrderLineDto ToDto(SalesOrderLine line) => new()
    {
        Id = line.Id,
        ProductId = line.ProductId,
        ProductVariantId = line.ProductVariantId,
        SkuSnapshot = line.SkuSnapshot,
        DescriptionSnapshot = line.DescriptionSnapshot,
        Quantity = line.Quantity,
        UnitPrice = line.UnitPrice,
        TaxRateSnapshot = line.TaxRateSnapshot,
        TaxTypeSnapshot = line.TaxTypeSnapshot,
        DiscountAmount = line.DiscountAmount,
        TaxAmount = line.TaxAmount,
        LineTotal = line.LineTotal,
        CostPriceSnapshot = line.CostPriceSnapshot,
        QuantityReturned = line.QuantityReturned,
    };
}
