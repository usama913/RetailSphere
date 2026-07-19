using RetailSphere.Contracts.Sales;
using RetailSphere.Domain.Sales;

namespace RetailSphere.Application.Features.SalesReturns;

internal static class SalesReturnMappings
{
    public static SalesReturnDto ToDto(
        SalesReturn salesReturn, string? salesOrderNumber, string? branchName, string? customerName, string? processedByUserName) => new()
    {
        Id = salesReturn.Id,
        ReturnNumber = salesReturn.ReturnNumber,
        SalesOrderId = salesReturn.SalesOrderId,
        SalesOrderNumber = salesOrderNumber,
        BranchId = salesReturn.BranchId,
        BranchName = branchName,
        CustomerId = salesReturn.CustomerId,
        CustomerName = customerName,
        ProcessedByUserId = salesReturn.ProcessedByUserId,
        ProcessedByUserName = processedByUserName,
        ReturnDate = salesReturn.ReturnDate,
        Reason = salesReturn.Reason,
        RefundAmount = salesReturn.RefundAmount,
        Lines = salesReturn.Lines.Select(ToDto).ToList(),
    };

    public static SalesReturnLineDto ToDto(SalesReturnLine line) => new()
    {
        Id = line.Id,
        SalesOrderLineId = line.SalesOrderLineId,
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
        RefundAmount = line.RefundAmount,
    };
}
