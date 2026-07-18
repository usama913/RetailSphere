using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.TransferStock;

public sealed record TransferStockCommand(
    long ProductVariantId,
    long FromBranchId,
    long ToBranchId,
    decimal Quantity,
    string Reason) : IRequest<Result>;
