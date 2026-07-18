using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Suppliers.DeleteSupplier;

public sealed class DeleteSupplierCommandHandler(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<DeleteSupplierCommand, Result>
{
    public async Task<Result> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        if (supplier is null)
            return Result.Failure(Error.NotFound("Supplier.NotFound", "Supplier not found."));

        supplierRepository.Remove(supplier);
        auditLogService.Log("Supplier", supplier.Id.ToString(), "Deleted", $"Deleted supplier '{supplier.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
