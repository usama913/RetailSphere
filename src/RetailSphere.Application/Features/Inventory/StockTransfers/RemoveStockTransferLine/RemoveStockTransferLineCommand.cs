using MediatR;
using RetailSphere.Contracts.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.RemoveStockTransferLine;

public sealed record RemoveStockTransferLineCommand(long StockTransferId, long LineId) : IRequest<Result<StockTransferDto>>;
