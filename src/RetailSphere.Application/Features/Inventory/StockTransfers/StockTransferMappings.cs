using RetailSphere.Contracts.Inventory;
using RetailSphere.Domain.Inventory;

namespace RetailSphere.Application.Features.Inventory.StockTransfers;

internal static class StockTransferMappings
{
    public static StockTransferDto ToDto(StockTransfer stockTransfer, string? fromBranchName, string? toBranchName) => new()
    {
        Id = stockTransfer.Id,
        TransferNumber = stockTransfer.TransferNumber,
        FromBranchId = stockTransfer.FromBranchId,
        FromBranchName = fromBranchName,
        ToBranchId = stockTransfer.ToBranchId,
        ToBranchName = toBranchName,
        Status = stockTransfer.Status,
        TransferDate = stockTransfer.TransferDate,
        Notes = stockTransfer.Notes,
        TotalQuantityRequested = stockTransfer.TotalQuantityRequested,
        Lines = stockTransfer.Lines.Select(ToDto).ToList(),
    };

    public static StockTransferLineDto ToDto(StockTransferLine line) => new()
    {
        Id = line.Id,
        ProductId = line.ProductId,
        ProductVariantId = line.ProductVariantId,
        SkuSnapshot = line.SkuSnapshot,
        DescriptionSnapshot = line.DescriptionSnapshot,
        QuantityRequested = line.QuantityRequested,
        QuantityReceived = line.QuantityReceived,
        RemainingQuantity = line.RemainingQuantity,
        IsFullyReceived = line.IsFullyReceived,
    };
}
