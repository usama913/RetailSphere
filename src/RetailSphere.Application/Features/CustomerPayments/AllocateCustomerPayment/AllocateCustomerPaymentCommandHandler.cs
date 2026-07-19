using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.CustomerPayments.Common;
using RetailSphere.Contracts.Customers;
using RetailSphere.Domain.Customers;
using RetailSphere.Domain.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerPayments.AllocateCustomerPayment;

/// <summary>Applies part of a payment's still-unallocated (on-account) balance to a sales order raised after the payment was originally recorded.</summary>
public sealed class AllocateCustomerPaymentCommandHandler(
    ICustomerPaymentRepository customerPaymentRepository,
    ISalesOrderRepository salesOrderRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    CustomerPaymentDtoAssembler customerPaymentDtoAssembler)
    : IRequestHandler<AllocateCustomerPaymentCommand, Result<CustomerPaymentDto>>
{
    public async Task<Result<CustomerPaymentDto>> Handle(AllocateCustomerPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await customerPaymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment is null)
            return Result.Failure<CustomerPaymentDto>(Error.NotFound("CustomerPayment.NotFound", "Customer payment not found."));

        var order = await salesOrderRepository.GetByIdAsync(request.SalesOrderId, cancellationToken);
        if (order is null)
            return Result.Failure<CustomerPaymentDto>(Error.NotFound("SalesOrder.NotFound", "Sales order not found."));

        if (order.CustomerId != payment.CustomerId)
            return Result.Failure<CustomerPaymentDto>(Error.Validation("CustomerPayment.CustomerMismatch", "This sales order does not belong to the same customer as the payment."));

        var allocateResult = payment.Allocate(request.SalesOrderId, request.Amount);
        if (allocateResult.IsFailure)
            return Result.Failure<CustomerPaymentDto>(allocateResult.Error);

        var applyResult = order.RecordAdditionalPayment(request.Amount);
        if (applyResult.IsFailure)
            return Result.Failure<CustomerPaymentDto>(applyResult.Error);

        customerPaymentRepository.Update(payment);
        salesOrderRepository.Update(order);
        auditLogService.Log(
            "CustomerPayment", payment.Id.ToString(), "Allocated",
            $"Applied {request.Amount:0.00} of this payment to sales order '{order.OrderNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await customerPaymentDtoAssembler.ToDtoAsync(payment, cancellationToken);
        return Result.Success(dto);
    }
}
