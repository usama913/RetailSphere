using MediatR;
using RetailSphere.Contracts.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.ReceiveStockTransferLine;

public sealed record ReceiveStockTransferLineCommand(long StockTransferId, long LineId, decimal Quantity) : IRequest<Result<StockTransferDto>>;
