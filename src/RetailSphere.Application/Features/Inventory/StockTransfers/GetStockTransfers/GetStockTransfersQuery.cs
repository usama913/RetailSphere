using MediatR;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.GetStockTransfers;

public sealed record GetStockTransfersQuery(
    int Page,
    int PageSize,
    string? Search,
    long? FromBranchId,
    long? ToBranchId,
    string? Status) : IRequest<Result<PagedResult<StockTransferDto>>>;
