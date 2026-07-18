using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Suppliers.UpdateSupplier;

public sealed class UpdateSupplierCommandHandler(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<UpdateSupplierCommand, Result<SupplierDto>>
{
    public async Task<Result<SupplierDto>> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        if (supplier is null)
            return Result.Failure<SupplierDto>(Error.NotFound("Supplier.NotFound", "Supplier not found."));

        var updateResult = supplier.UpdateDetails(request.Name, request.ContactPerson, request.Email, request.Phone, request.Address, request.TaxNumber);
        if (updateResult.IsFailure)
            return Result.Failure<SupplierDto>(updateResult.Error);

        supplierRepository.Update(supplier);
        auditLogService.Log("Supplier", supplier.Id.ToString(), "Updated", $"Updated supplier '{supplier.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(SupplierMappings.ToDto(supplier));
    }
}
