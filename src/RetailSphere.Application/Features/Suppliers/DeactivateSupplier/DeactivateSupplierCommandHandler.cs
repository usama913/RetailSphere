using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Suppliers.DeactivateSupplier;

public sealed class DeactivateSupplierCommandHandler(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<DeactivateSupplierCommand, Result>
{
    public async Task<Result> Handle(DeactivateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        if (supplier is null)
            return Result.Failure(Error.NotFound("Supplier.NotFound", "Supplier not found."));

        supplier.Deactivate();
        supplierRepository.Update(supplier);
        auditLogService.Log("Supplier", supplier.Id.ToString(), "Deactivated", $"Deactivated supplier '{supplier.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
