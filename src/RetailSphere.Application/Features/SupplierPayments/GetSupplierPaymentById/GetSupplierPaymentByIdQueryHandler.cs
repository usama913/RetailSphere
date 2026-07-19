using MediatR;
using RetailSphere.Application.Features.SupplierPayments.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SupplierPayments.GetSupplierPaymentById;

public sealed class GetSupplierPaymentByIdQueryHandler(ISupplierPaymentRepository supplierPaymentRepository, SupplierPaymentDtoAssembler supplierPaymentDtoAssembler)
    : IRequestHandler<GetSupplierPaymentByIdQuery, Result<SupplierPaymentDto>>
{
    public async Task<Result<SupplierPaymentDto>> Handle(GetSupplierPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await supplierPaymentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (payment is null)
            return Result.Failure<SupplierPaymentDto>(Error.NotFound("SupplierPayment.NotFound", "Supplier payment not found."));

        var dto = await supplierPaymentDtoAssembler.ToDtoAsync(payment, cancellationToken);
        return Result.Success(dto);
    }
}
