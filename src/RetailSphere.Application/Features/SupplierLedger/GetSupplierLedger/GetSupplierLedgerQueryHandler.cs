using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SupplierLedger.GetSupplierLedger;

/// <summary>
/// Builds the Supplier Ledger: every invoice (debit — increases what we owe) and
/// every non-reversed payment (credit — decreases what we owe) for one supplier,
/// sorted chronologically with a running balance. Reversed payments are still
/// listed (audit transparency — the Notifications & Alerts / Audit Trail
/// requirements call for every financial transaction being traceable) but
/// contribute nothing to the running balance, since the linked PurchaseInvoice's
/// AmountPaid was already given back at reversal time.
/// </summary>
public sealed class GetSupplierLedgerQueryHandler(
    ISupplierRepository supplierRepository,
    IPurchaseInvoiceRepository purchaseInvoiceRepository,
    ISupplierPaymentRepository supplierPaymentRepository)
    : IRequestHandler<GetSupplierLedgerQuery, Result<SupplierLedgerDto>>
{
    public async Task<Result<SupplierLedgerDto>> Handle(GetSupplierLedgerQuery request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier is null)
            return Result.Failure<SupplierLedgerDto>(Error.NotFound("Supplier.NotFound", "Supplier not found."));

        var invoices = await purchaseInvoiceRepository.GetBySupplierAsync(request.SupplierId, cancellationToken);
        var payments = await supplierPaymentRepository.GetBySupplierAsync(request.SupplierId, cancellationToken);

        var rows = new List<(DateTime Date, string Type, string? Reference, string Description, decimal Debit, decimal Credit, bool IsReversed)>();

        foreach (var invoice in invoices)
        {
            rows.Add((
                invoice.InvoiceDate,
                "Invoice",
                invoice.SupplierInvoiceNumber,
                string.IsNullOrWhiteSpace(invoice.Notes) ? $"Purchase invoice {invoice.SupplierInvoiceNumber}" : invoice.Notes,
                invoice.TotalAmount,
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
        var entries = new List<SupplierLedgerEntryDto>();
        foreach (var row in rows.OrderBy(r => r.Date))
        {
            running += row.Debit - row.Credit;
            entries.Add(new SupplierLedgerEntryDto
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

        return Result.Success(new SupplierLedgerDto
        {
            SupplierId = supplier.Id,
            SupplierName = supplier.Name,
            ClosingBalance = running,
            Entries = entries,
        });
    }
}
