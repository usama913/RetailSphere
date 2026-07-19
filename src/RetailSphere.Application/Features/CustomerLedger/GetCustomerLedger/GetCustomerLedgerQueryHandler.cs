using MediatR;
using RetailSphere.Contracts.Customers;
using RetailSphere.Domain.Customers;
using RetailSphere.Domain.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerLedger.GetCustomerLedger;

/// <summary>
/// Builds the Customer Ledger: every sale (debit — increases what the customer owes)
/// and every non-reversed payment (credit — decreases it), sorted chronologically
/// with a running balance. Mirrors GetSupplierLedgerQueryHandler on the AP side.
/// Sales Returns aren't itemized as their own ledger row — the current domain model
/// only tracks a return's effect through SalesOrderLine.QuantityReturned, not a
/// separate AmountPaid/credit-note adjustment, so there is nothing distinct to post
/// here yet; a future SalesReturn-issues-a-credit-note feature would add a third
/// entry type alongside Sale/Payment.
/// </summary>
public sealed class GetCustomerLedgerQueryHandler(
    ICustomerRepository customerRepository,
    ISalesOrderRepository salesOrderRepository,
    ICustomerPaymentRepository customerPaymentRepository)
    : IRequestHandler<GetCustomerLedgerQuery, Result<CustomerLedgerDto>>
{
    public async Task<Result<CustomerLedgerDto>> Handle(GetCustomerLedgerQuery request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
            return Result.Failure<CustomerLedgerDto>(Error.NotFound("Customer.NotFound", "Customer not found."));

        var orders = await salesOrderRepository.GetByCustomerAsync(request.CustomerId, cancellationToken);
        var payments = await customerPaymentRepository.GetByCustomerAsync(request.CustomerId, cancellationToken);

        var rows = new List<(DateTime Date, string Type, string? Reference, string Description, decimal Debit, decimal Credit, bool IsReversed)>();

        foreach (var order in orders.Where(o => o.Status != "Cancelled"))
        {
            rows.Add((
                order.OrderDate,
                "Sale",
                order.OrderNumber,
                $"Sale {order.OrderNumber} ({order.PaymentMethod})",
                order.TotalAmount,
                0m,
                false));
        }

        foreach (var payment in payments)
        {
            if (payment.IsReversed)
            {
                rows.Add((
                    payment.ReversedAtUtc ?? payment.PaymentDate,
                    "Payment",
                    payment.ReferenceNumber,
                    $"Payment via {payment.PaymentMethod} (reversed: {payment.ReversalReason})",
                    0m,
                    0m,
                    true));
            }
            else
            {
                rows.Add((
                    payment.PaymentDate,
                    "Payment",
                    payment.ReferenceNumber,
                    $"Payment via {payment.PaymentMethod}",
                    0m,
                    payment.Amount,
                    false));
            }
        }

        var running = 0m;
        var entries = new List<CustomerLedgerEntryDto>();
        foreach (var row in rows.OrderBy(r => r.Date))
        {
            running += row.Debit - row.Credit;
            entries.Add(new CustomerLedgerEntryDto
            {
                Date = row.Date,
                Type = row.Type,
                ReferenceNumber = row.Reference,
                Description = row.Description,
                DebitAmount = row.Debit,
                CreditAmount = row.Credit,
                RunningBalance = running,
                IsReversed = row.IsReversed,
            });
        }

        return Result.Success(new CustomerLedgerDto
        {
            CustomerId = customer.Id,
            CustomerName = customer.Name,
            ClosingBalance = running,
            Entries = entries,
        });
    }
}
