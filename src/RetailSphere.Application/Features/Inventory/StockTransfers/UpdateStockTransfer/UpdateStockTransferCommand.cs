using MediatR;
using RetailSphere.Contracts.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.UpdateStockTransfer;

public sealed record UpdateStockTransferCommand(
    long Id,
    long FromBranchId,
    long ToBranchId,
    DateTime TransferDate,
    string? Notes) : IRequest<Result<StockTransferDto>>;
