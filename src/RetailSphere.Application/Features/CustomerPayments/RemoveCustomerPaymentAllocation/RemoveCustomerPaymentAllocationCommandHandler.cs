using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.CustomerPayments.Common;
using RetailSphere.Contracts.Customers;
using RetailSphere.Domain.Customers;
using RetailSphere.Domain.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerPayments.RemoveCustomerPaymentAllocation;

/// <summary>Undoes one allocation — gives the amount back to the sales order's outstanding balance and back to the payment's unallocated pool.</summary>
public sealed class RemoveCustomerPaymentAllocationCommandHandler(
    ICustomerPaymentRepository customerPaymentRepository,
    ISalesOrderRepository salesOrderRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    CustomerPaymentDtoAssembler customerPaymentDtoAssembler)
    : IRequestHandler<RemoveCustomerPaymentAllocationCommand, Result<CustomerPaymentDto>>
{
    public async Task<Result<CustomerPaymentDto>> Handle(RemoveCustomerPaymentAllocationCommand request, CancellationToken cancellationToken)
    {
        var payment = await customerPaymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment is null)
            return Result.Failure<CustomerPaymentDto>(Error.NotFound("CustomerPayment.NotFound", "Customer payment not found."));

        var removeResult = payment.RemoveAllocation(request.SalesOrderId);
        if (removeResult.IsFailure)
            return Result.Failure<CustomerPaymentDto>(removeResult.Error);

        var removedAmount = removeResult.Value;

        var order = await salesOrderRepository.GetByIdAsync(request.SalesOrderId, cancellationToken);
        if (order is not null)
        {
            var reverseResult = order.ReverseAdditionalPayment(removedAmount);
            if (reverseResult.IsFailure)
                return Result.Failure<CustomerPaymentDto>(reverseResult.Error);

            salesOrderRepository.Update(order);
        }

        customerPaymentRepository.Update(payment);
        auditLogService.Log(
            "CustomerPayment", payment.Id.ToString(), "AllocationRemoved",
            $"Removed a {removedAmount:0.00} allocation from sales order {request.SalesOrderId}.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await customerPaymentDtoAssembler.ToDtoAsync(payment, cancellationToken);
        return Result.Success(dto);
    }
}
