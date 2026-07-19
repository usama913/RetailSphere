using MediatR;
using RetailSphere.Contracts.Reporting;
using RetailSphere.Domain.Customers;
using RetailSphere.Domain.Purchasing;
using RetailSphere.Domain.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Reporting.GetFinancialSummary;

/// <summary>
/// One-shot aggregation for the Dashboard Improvements section — Total Supplier/
/// Customer Outstanding, Overdue Supplier Payments/Customer Receivables, Today's
/// Collections/Supplier Payments, Cash Flow, A/R, A/P — computed fresh on every
/// call rather than cached, since it's cheap (a handful of already-indexed queries)
/// and always needs to be current for a dashboard.
/// </summary>
public sealed class GetFinancialSummaryQueryHandler(
    IPurchaseInvoiceRepository purchaseInvoiceRepository,
    ISalesOrderRepository salesOrderRepository,
    ISupplierPaymentRepository supplierPaymentRepository,
    ICustomerPaymentRepository customerPaymentRepository)
    : IRequestHandler<GetFinancialSummaryQuery, Result<FinancialSummaryDto>>
{
    public async Task<Result<FinancialSummaryDto>> Handle(GetFinancialSummaryQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var endOfToday = today.AddDays(1).AddTicks(-1);

        var outstandingInvoices = await purchaseInvoiceRepository.GetOutstandingAsync(supplierId: null, cancellationToken);
        var overdueInvoices = outstandingInvoices.Where(i => i.DueDate.Date < today).ToList();

        var outstandingOrders = await salesOrderRepository.GetOutstandingAsync(cancellationToken);
        var overdueOrders = outstandingOrders.Where(o => (o.DueDate ?? o.OrderDate).Date < today).ToList();

        var (todaysSupplierPayments, _) = await supplierPaymentRepository.SearchAsync(1, 100000, null, null, today, endOfToday, cancellationToken);
        var (todaysCustomerPayments, _) = await customerPaymentRepository.SearchAsync(1, 100000, null, null, today, endOfToday, cancellationToken);

        var todaysCollections = todaysCustomerPayments.Where(p => !p.IsReversed).Sum(p => p.Amount);
        var todaysSupplierPaymentTotal = todaysSupplierPayments.Where(p => !p.IsReversed).Sum(p => p.Amount);

        return Result.Success(new FinancialSummaryDto
        {
            GeneratedAtUtc = DateTime.UtcNow,
            AccountsPayable = outstandingInvoices.Sum(i => i.OutstandingBalance),
            AccountsReceivable = outstandingOrders.Sum(o => o.OutstandingBalance),
            OverdueSupplierAmount = overdueInvoices.Sum(i => i.OutstandingBalance),
            OverdueSupplierInvoiceCount = overdueInvoices.Count,
            OverdueCustomerAmount = overdueOrders.Sum(o => o.OutstandingBalance),
            OverdueCustomerInvoiceCount = overdueOrders.Count,
            TodaysCollections = todaysCollections,
            TodaysSupplierPayments = todaysSupplierPaymentTotal,
            NetCashFlowToday = todaysCollections - todaysSupplierPaymentTotal,
        });
    }
}
