using MediatR;
using RetailSphere.Contracts.Customers;
using RetailSphere.Domain.Customers;
using RetailSphere.Domain.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerLedger.GetCustomerAgingReport;

/// <summary>Buckets every outstanding Sales Order by days past its DueDate, grouped by customer — mirrors GetSupplierAgingReportQueryHandler.</summary>
public sealed class GetCustomerAgingReportQueryHandler(ISalesOrderRepository salesOrderRepository, ICustomerRepository customerRepository)
    : IRequestHandler<GetCustomerAgingReportQuery, Result<CustomerAgingReportDto>>
{
    public async Task<Result<CustomerAgingReportDto>> Handle(GetCustomerAgingReportQuery request, CancellationToken cancellationToken)
    {
        var outstandingOrders = await salesOrderRepository.GetOutstandingAsync(cancellationToken);
        var customers = (await customerRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(c => c.Id, c => c.Name);

        var today = DateTime.UtcNow.Date;

        var buckets = outstandingOrders
            .Where(o => o.CustomerId.HasValue)
            .GroupBy(o => o.CustomerId!.Value)
            .Select(group =>
            {
                decimal current = 0, days30 = 0, days60 = 0, days90 = 0, over90 = 0;

                foreach (var order in group)
                {
                    var dueDate = order.DueDate ?? order.OrderDate;
                    var daysPastDue = (today - dueDate.Date).Days;
                    var amount = order.OutstandingBalance;

                    if (daysPastDue <= 0)
                        current += amount;
                    else if (daysPastDue <= 30)
                        days30 += amount;
                    else if (daysPastDue <= 60)
                        days60 += amount;
                    else if (daysPastDue <= 90)
                        days90 += amount;
                    else
                        over90 += amount;
                }

                return new CustomerAgingBucketDto
                {
                    CustomerId = group.Key,
                    CustomerName = customers.TryGetValue(group.Key, out var name) ? name : "Unknown Customer",
                    Current = current,
                    Days1To30 = days30,
                    Days31To60 = days60,
                    Days61To90 = days90,
                    Over90Days = over90,
                    Total = current + days30 + days60 + days90 + over90,
                };
            })
            .OrderByDescending(b => b.Total)
            .ToList();

        return Result.Success(new CustomerAgingReportDto
        {
            GeneratedAtUtc = DateTime.UtcNow,
            Customers = buckets,
            GrandTotal = buckets.Sum(b => b.Total),
        });
    }
}
