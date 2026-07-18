using MediatR;
using RetailSphere.Contracts.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.GetStockItemById;

public sealed record GetStockItemByIdQuery(long Id) : IRequest<Result<StockItemDto>>;
