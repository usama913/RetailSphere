using MediatR;
using RetailSphere.Contracts.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.AdjustStock;

public sealed record AdjustStockCommand(
    long ProductVariantId,
    long BranchId,
    decimal QuantityDelta,
    string Reason) : IRequest<Result<StockItemDto>>;
