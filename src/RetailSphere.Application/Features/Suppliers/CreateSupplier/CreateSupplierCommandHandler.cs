using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Suppliers.CreateSupplier;

public sealed class CreateSupplierCommandHandler(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<CreateSupplierCommand, Result<SupplierDto>>
{
    public async Task<Result<SupplierDto>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplierResult = Supplier.Create(request.Name, request.ContactPerson, request.Email, request.Phone, request.Address, request.TaxNumber);
        if (supplierResult.IsFailure)
            return Result.Failure<SupplierDto>(supplierResult.Error);

        var supplier = supplierResult.Value;
        supplierRepository.Add(supplier);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        auditLogService.Log("Supplier", supplier.Id.ToString(), "Created", $"Created supplier '{supplier.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(SupplierMappings.ToDto(supplier));
    }
}
