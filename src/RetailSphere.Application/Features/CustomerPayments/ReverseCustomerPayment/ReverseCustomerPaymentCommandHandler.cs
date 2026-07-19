using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.CustomerPayments.Common;
using RetailSphere.Common;
using RetailSphere.Contracts.Customers;
using RetailSphere.Domain.Customers;
using RetailSphere.Domain.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerPayments.ReverseCustomerPayment;

/// <summary>
/// Reversing a payment must also give back every Sales Order it was allocated to —
/// walks the payment's Allocations (still present after Reverse() flips IsReversed,
/// since Reverse() intentionally leaves them in place as a historical record) and
/// calls ReverseAdditionalPayment for each one's amount, in the same unit of work.
/// </summary>
public sealed class ReverseCustomerPaymentCommandHandler(
    ICustomerPaymentRepository customerPaymentRepository,
    ISalesOrderRepository salesOrderRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    CustomerPaymentDtoAssembler customerPaymentDtoAssembler,
    ICurrentUserService currentUserService)
    : IRequestHandler<ReverseCustomerPaymentCommand, Result<CustomerPaymentDto>>
{
    public async Task<Result<CustomerPaymentDto>> Handle(ReverseCustomerPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await customerPaymentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (payment is null)
            return Result.Failure<CustomerPaymentDto>(Error.NotFound("CustomerPayment.NotFound", "Customer payment not found."));

        var allocations = payment.Allocations.ToList();

        var reverseResult = payment.Reverse(request.Reason, currentUserService.UserId);
        if (reverseResult.IsFailure)
            return Result.Failure<CustomerPaymentDto>(reverseResult.Error);

        var touchedOrders = new List<SalesOrder>();
        foreach (var allocation in allocations)
        {
            var order = await salesOrderRepository.GetByIdAsync(allocation.SalesOrderId, cancellationToken);
            if (order is null)
                continue;

            var orderReverseResult = order.ReverseAdditionalPayment(allocation.Amount);
            if (orderReverseResult.IsFailure)
                return Result.Failure<CustomerPaymentDto>(orderReverseResult.Error);

            touchedOrders.Add(order);
        }

        customerPaymentRepository.Update(payment);
        foreach (var order in touchedOrders)
            salesOrderRepository.Update(order);

        auditLogService.Log(
            "CustomerPayment", payment.Id.ToString(), "Reversed",
            $"Reversed payment of {payment.Amount:0.00} — reason: {request.Reason}");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await customerPaymentDtoAssembler.ToDtoAsync(payment, cancellationToken);
        return Result.Success(dto);
    }
}
