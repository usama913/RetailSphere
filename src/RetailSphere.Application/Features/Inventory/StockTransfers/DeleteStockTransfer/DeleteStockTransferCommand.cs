using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.DeleteStockTransfer;

public sealed record DeleteStockTransferCommand(long Id) : IRequest<Result>;
