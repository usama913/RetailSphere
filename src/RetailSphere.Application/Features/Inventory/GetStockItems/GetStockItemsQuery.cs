using MediatR;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.GetStockItems;

public sealed record GetStockItemsQuery(
    int Page,
    int PageSize,
    long? BranchId,
    long? ProductId) : IRequest<Result<PagedResult<StockItemDto>>>;
