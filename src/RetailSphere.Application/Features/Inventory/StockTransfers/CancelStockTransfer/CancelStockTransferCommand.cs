using MediatR;
using RetailSphere.Contracts.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.CancelStockTransfer;

public sealed record CancelStockTransferCommand(long Id) : IRequest<Result<StockTransferDto>>;
