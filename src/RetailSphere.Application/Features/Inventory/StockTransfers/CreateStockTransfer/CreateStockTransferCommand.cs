using MediatR;
using RetailSphere.Contracts.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.CreateStockTransfer;

public sealed record CreateStockTransferCommand(
    long FromBranchId,
    long ToBranchId,
    DateTime TransferDate,
    string? Notes) : IRequest<Result<StockTransferDto>>;
