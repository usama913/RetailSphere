using MediatR;
using RetailSphere.Contracts.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.AddStockTransferLine;

public sealed record AddStockTransferLineCommand(
    long StockTransferId,
    long ProductId,
    long ProductVariantId,
    decimal QuantityRequested) : IRequest<Result<StockTransferDto>>;
