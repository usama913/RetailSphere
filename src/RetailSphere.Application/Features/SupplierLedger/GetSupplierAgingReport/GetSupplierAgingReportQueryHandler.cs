using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SupplierLedger.GetSupplierAgingReport;

/// <summary>
/// Buckets every outstanding Purchase Invoice by how many days past its due date it
/// is — Current (not yet due), 1-30, 31-60, 61-90, 90+ — grouped by supplier. Feeds
/// both the standalone Supplier Aging report and the "Total Supplier Outstanding" /
/// "Overdue Supplier Payments" dashboard widgets (they can just sum this report's
/// buckets rather than re-querying).
/// </summary>
public sealed class GetSupplierAgingReportQueryHandler(IPurchaseInvoiceRepository purchaseInvoiceRepository, ISupplierRepository supplierRepository)
    : IRequestHandler<GetSupplierAgingReportQuery, Result<SupplierAgingReportDto>>
{
    public async Task<Result<SupplierAgingReportDto>> Handle(GetSupplierAgingReportQuery request, CancellationToken cancellationToken)
    {
        var outstandingInvoices = await purchaseInvoiceRepository.GetOutstandingAsync(request.SupplierId, cancellationToken);
        var suppliers = (await supplierRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(s => s.Id, s => s.Name);

        var today = DateTime.UtcNow.Date;

        var buckets = outstandingInvoices
            .GroupBy(i => i.SupplierId)
            .Select(group =>
            {
                decimal current = 0, days30 = 0, days60 = 0, days90 = 0, over90 = 0;

                foreach (var invoice in group)
                {
                    var daysPastDue = (today - invoice.DueDate.Date).Days;
                    var amount = invoice.OutstandingBalance;

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

                return new SupplierAgingBucketDto
                {
                    SupplierId = group.Key,
                    SupplierName = suppliers.TryGetValue(group.Key, out var name) ? name : "Unknown Supplier",
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

        return Result.Success(new SupplierAgingReportDto
        {
            GeneratedAtUtc = DateTime.UtcNow,
            Suppliers = buckets,
            GrandTotal = buckets.Sum(b => b.Total),
        });
    }
}
