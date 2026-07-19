using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.CustomerPayments.Common;
using RetailSphere.Contracts.Customers;
using RetailSphere.Domain.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerPayments.UpdateCustomerPayment;

public sealed class UpdateCustomerPaymentCommandHandler(
    ICustomerPaymentRepository customerPaymentRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    CustomerPaymentDtoAssembler customerPaymentDtoAssembler)
    : IRequestHandler<UpdateCustomerPaymentCommand, Result<CustomerPaymentDto>>
{
    public async Task<Result<CustomerPaymentDto>> Handle(UpdateCustomerPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await customerPaymentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (payment is null)
            return Result.Failure<CustomerPaymentDto>(Error.NotFound("CustomerPayment.NotFound", "Customer payment not found."));

        var updateResult = payment.UpdateDetails(request.PaymentDate, request.Amount, request.PaymentMethod, request.ReferenceNumber, request.Notes);
        if (updateResult.IsFailure)
            return Result.Failure<CustomerPaymentDto>(updateResult.Error);

        customerPaymentRepository.Update(payment);
        auditLogService.Log("CustomerPayment", payment.Id.ToString(), "Updated", $"Edited customer payment to {payment.Amount:0.00}.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await customerPaymentDtoAssembler.ToDtoAsync(payment, cancellationToken);
        return Result.Success(dto);
    }
}
