using MediatR;
using RetailSphere.Contracts.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.GetStockTransferById;

public sealed record GetStockTransferByIdQuery(long Id) : IRequest<Result<StockTransferDto>>;
