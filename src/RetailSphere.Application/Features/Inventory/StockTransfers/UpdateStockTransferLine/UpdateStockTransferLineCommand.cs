using MediatR;
using RetailSphere.Contracts.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.UpdateStockTransferLine;

public sealed record UpdateStockTransferLineCommand(
    long StockTransferId,
    long LineId,
    decimal QuantityRequested) : IRequest<Result<StockTransferDto>>;
