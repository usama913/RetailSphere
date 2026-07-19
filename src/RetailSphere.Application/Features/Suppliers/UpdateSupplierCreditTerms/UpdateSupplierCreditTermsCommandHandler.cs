using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Suppliers.UpdateSupplierCreditTerms;

public sealed class UpdateSupplierCreditTermsCommandHandler(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<UpdateSupplierCreditTermsCommand, Result<SupplierDto>>
{
    public async Task<Result<SupplierDto>> Handle(UpdateSupplierCreditTermsCommand request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.Id, cancellationToken);
        if (supplier is null)
            return Result.Failure<SupplierDto>(Error.NotFound("Supplier.NotFound", "Supplier not found."));

        var updateResult = supplier.UpdateCreditTerms(request.CreditLimit, request.PaymentTerms);
        if (updateResult.IsFailure)
            return Result.Failure<SupplierDto>(updateResult.Error);

        supplierRepository.Update(supplier);
        auditLogService.Log(
            "Supplier", supplier.Id.ToString(), "CreditTermsUpdated",
            $"Set credit terms for '{supplier.Name}': limit {(request.CreditLimit.HasValue ? request.CreditLimit.Value.ToString("0.00") : "unlimited")}, terms {supplier.PaymentTerms}.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(SupplierMappings.ToDto(supplier));
    }
}
