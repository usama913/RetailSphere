using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.CustomerPayments.Common;
using RetailSphere.Contracts.Customers;
using RetailSphere.Domain.Customers;
using RetailSphere.Domain.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerPayments.RecordCustomerPayment;

/// <summary>
/// Records a customer payment and, in the same request, applies it against zero or
/// more of that customer's outstanding sales orders — a single cheque/transfer can
/// clear several invoices at once (per the Customer Payment Module spec). Any
/// remaining unallocated amount is kept on the payment as an on-account credit that
/// a later AllocateCustomerPayment call can apply.
/// </summary>
public sealed class RecordCustomerPaymentCommandHandler(
    ICustomerRepository customerRepository,
    ISalesOrderRepository salesOrderRepository,
    ICustomerPaymentRepository customerPaymentRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    CustomerPaymentDtoAssembler customerPaymentDtoAssembler)
    : IRequestHandler<RecordCustomerPaymentCommand, Result<CustomerPaymentDto>>
{
    public async Task<Result<CustomerPaymentDto>> Handle(RecordCustomerPaymentCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
            return Result.Failure<CustomerPaymentDto>(Error.NotFound("Customer.NotFound", "Customer not found."));

        var paymentResult = CustomerPayment.Create(
            request.CustomerId, request.BranchId, request.PaymentDate, request.Amount, request.PaymentMethod, request.ReferenceNumber, request.Notes);
        if (paymentResult.IsFailure)
            return Result.Failure<CustomerPaymentDto>(paymentResult.Error);

        var payment = paymentResult.Value;
        var touchedOrders = new List<SalesOrder>();

        foreach (var allocation in request.Allocations)
        {
            var order = await salesOrderRepository.GetByIdAsync(allocation.SalesOrderId, cancellationToken);
            if (order is null)
                return Result.Failure<CustomerPaymentDto>(Error.NotFound("SalesOrder.NotFound", $"Sales order {allocation.SalesOrderId} not found."));

            if (order.CustomerId != request.CustomerId)
                return Result.Failure<CustomerPaymentDto>(Error.Validation("CustomerPayment.CustomerMismatch", "One of the selected sales orders does not belong to this customer."));

            var allocateResult = payment.Allocate(allocation.SalesOrderId, allocation.Amount);
            if (allocateResult.IsFailure)
                return Result.Failure<CustomerPaymentDto>(allocateResult.Error);

            var applyResult = order.RecordAdditionalPayment(allocation.Amount);
            if (applyResult.IsFailure)
                return Result.Failure<CustomerPaymentDto>(applyResult.Error);

            touchedOrders.Add(order);
        }

        customerPaymentRepository.Add(payment);
        foreach (var order in touchedOrders)
            salesOrderRepository.Update(order);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        auditLogService.Log(
            "CustomerPayment", payment.Id.ToString(), "Created",
            $"Recorded payment of {payment.Amount:0.00} from customer '{customer.Name}' ({touchedOrders.Count} invoice(s) allocated, {payment.UnallocatedAmount:0.00} unapplied).");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await customerPaymentDtoAssembler.ToDtoAsync(payment, cancellationToken);
        return Result.Success(dto);
    }
}
