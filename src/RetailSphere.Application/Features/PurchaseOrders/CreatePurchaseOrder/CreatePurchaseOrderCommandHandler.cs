using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.PurchaseOrders.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Organization;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.CreatePurchaseOrder;

public sealed class CreatePurchaseOrderCommandHandler(
    IPurchaseOrderRepository purchaseOrderRepository,
    ISupplierRepository supplierRepository,
    IBranchRepository branchRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    PurchaseOrderDtoAssembler purchaseOrderDtoAssembler)
    : IRequestHandler<CreatePurchaseOrderCommand, Result<PurchaseOrderDto>>
{
    private const int MaxPoNumberAttempts = 5;

    public async Task<Result<PurchaseOrderDto>> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier is null)
            return Result.Failure<PurchaseOrderDto>(Error.NotFound("Supplier.NotFound", "Supplier not found."));

        var branch = await branchRepository.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null)
            return Result.Failure<PurchaseOrderDto>(Error.NotFound("Branch.NotFound", "Branch not found."));

        // PoNumber generation checks for an existing number before inserting, but that
        // check-then-insert isn't atomic — two concurrent requests (e.g. a double-submit)
        // can both see the same number as free. Rather than only preventing that race,
        // this loop also recovers from it: if the unique-index insert still collides,
        // detach the failed transient entity, generate a fresh number, and retry.
        PurchaseOrder purchaseOrder;
        var attempt = 1;
        while (true)
        {
            var poNumber = await GeneratePoNumberAsync(cancellationToken);

            var purchaseOrderResult = PurchaseOrder.Create(poNumber, request.SupplierId, request.BranchId, request.OrderDate, request.ExpectedDeliveryDate, request.Notes);
            if (purchaseOrderResult.IsFailure)
                return Result.Failure<PurchaseOrderDto>(purchaseOrderResult.Error);

            purchaseOrder = purchaseOrderResult.Value;
            purchaseOrderRepository.Add(purchaseOrder);

            try
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                break;
            }
            catch (Exception ex) when (attempt < MaxPoNumberAttempts && IsDuplicatePoNumberViolation(ex))
            {
                // The entity is still in the Added state (the insert never actually
                // landed), so Remove() here just detaches it from tracking rather than
                // issuing a delete — safe to retry with a new number.
                purchaseOrderRepository.Remove(purchaseOrder);
                attempt++;
            }
        }

        auditLogService.Log("PurchaseOrder", purchaseOrder.Id.ToString(), "Created", $"Created purchase order '{purchaseOrder.PoNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await purchaseOrderDtoAssembler.ToDtoAsync(purchaseOrder, cancellationToken);
        return Result.Success(dto);
    }

    /// <summary>
    /// Detects a duplicate-PoNumber unique-index violation without taking a compile-time
    /// dependency on EF Core/the MySQL provider from this layer (Application must not
    /// reference EF Core — only Persistence does). Matched by type name and message text
    /// instead of catching Microsoft.EntityFrameworkCore.DbUpdateException directly; any
    /// exception that doesn't match this shape falls through and propagates normally.
    /// </summary>
    private static bool IsDuplicatePoNumberViolation(Exception ex) =>
        ex.GetType().Name == "DbUpdateException"
        && (ex.InnerException?.Message.Contains("IX_PurchaseOrders_PoNumber", StringComparison.OrdinalIgnoreCase) == true
            || ex.InnerException?.Message.Contains("Duplicate entry", StringComparison.OrdinalIgnoreCase) == true);

    /// <summary>
    /// Auto-generates a month-scoped PO number (e.g. "PO-202607-0001") with a
    /// uniqueness-retry loop — the same pattern as AddVariantCommandHandler's SKU
    /// generation, chosen so it doesn't need a two-phase save to learn the new
    /// aggregate's Id first.
    /// </summary>
    private async Task<string> GeneratePoNumberAsync(CancellationToken cancellationToken)
    {
        var prefix = $"PO-{DateTime.UtcNow:yyyyMM}-";
        var sequence = 1;
        while (true)
        {
            var candidate = $"{prefix}{sequence:0000}";
            if (!await purchaseOrderRepository.PoNumberExistsAsync(candidate, cancellationToken))
                return candidate;

            sequence++;
        }
    }
}
