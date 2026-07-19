using MediatR;
using RetailSphere.Contracts.Customers;
using RetailSphere.Domain.Customers;
using RetailSphere.Domain.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerLedger.GetCustomerCreditSummary;

/// <summary>
/// Powers the POS checkout's customer-selection panel (per the POS Integration
/// requirements): outstanding balance, available credit, unpaid invoice count,
/// last payment date, and total overdue — everything a cashier needs to decide
/// whether a credit/split sale should proceed or be flagged for override.
/// </summary>
public sealed class GetCustomerCreditSummaryQueryHandler(ICustomerRepository customerRepository, ISalesOrderRepository salesOrderRepository)
    : IRequestHandler<GetCustomerCreditSummaryQuery, Result<CustomerCreditSummaryDto>>
{
    public async Task<Result<CustomerCreditSummaryDto>> Handle(GetCustomerCreditSummaryQuery request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
            return Result.Failure<CustomerCreditSummaryDto>(Error.NotFound("Customer.NotFound", "Customer not found."));

        var outstandingOrders = await salesOrderRepository.GetOutstandingByCustomerAsync(request.CustomerId, cancellationToken);
        var allOrders = await salesOrderRepository.GetByCustomerAsync(request.CustomerId, cancellationToken);

        var today = DateTime.UtcNow.Date;
        var outstandingBalance = outstandingOrders.Sum(o => o.OutstandingBalance);
        var overdueAmount = outstandingOrders
            .Where(o => (o.DueDate ?? o.OrderDate).Date < today)
            .Sum(o => o.OutstandingBalance);

        var lastPaymentDate = allOrders
            .Where(o => o.AmountPaid > 0)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => (DateTime?)o.OrderDate)
            .FirstOrDefault();

        var availableCredit = customer.CreditLimit.HasValue ? customer.CreditLimit.Value - outstandingBalance : (decimal?)null;

        return Result.Success(new CustomerCreditSummaryDto
        {
            CustomerId = customer.Id,
            CustomerName = customer.Name,
            CreditLimit = customer.CreditLimit,
            CurrentOutstandingBalance = outstandingBalance,
            AvailableCredit = availableCredit,
            UnpaidInvoiceCount = outstandingOrders.Count,
            LastPaymentDate = lastPaymentDate,
            TotalOverdueAmount = overdueAmount,
            IsOverCreditLimit = customer.CreditLimit.HasValue && outstandingBalance > customer.CreditLimit.Value,
        });
    }
}
