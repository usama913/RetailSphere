using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Suppliers.ActivateSupplier;

public sealed class ActivateSupplierCommandHandler(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<ActivateSupplierCommand, Result>
{
    public async Task<Result> Handle(ActivateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        if (supplier is null)
            return Result.Failure(Error.NotFound("Supplier.NotFound", "Supplier not found."));

        supplier.Activate();
        supplierRepository.Update(supplier);
        auditLogService.Log("Supplier", supplier.Id.ToString(), "Activated", $"Activated supplier '{supplier.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
