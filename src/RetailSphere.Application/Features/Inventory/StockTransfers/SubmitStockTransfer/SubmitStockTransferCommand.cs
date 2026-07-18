using MediatR;
using RetailSphere.Contracts.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.SubmitStockTransfer;

public sealed record SubmitStockTransferCommand(long Id) : IRequest<Result<StockTransferDto>>;
