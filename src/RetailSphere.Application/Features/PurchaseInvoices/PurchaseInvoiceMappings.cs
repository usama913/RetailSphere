using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;

namespace RetailSphere.Application.Features.PurchaseInvoices;

internal static class PurchaseInvoiceMappings
{
    public static PurchaseInvoiceDto ToDto(PurchaseInvoice invoice, string? supplierName, string? branchName, string? purchaseOrderNumber) => new()
    {
        Id = invoice.Id,
        SupplierId = invoice.SupplierId,
        SupplierName = supplierName,
        BranchId = invoice.BranchId,
        BranchName = branchName,
        PurchaseOrderId = invoice.PurchaseOrderId,
        PurchaseOrderNumber = purchaseOrderNumber,
        SupplierInvoiceNumber = invoice.SupplierInvoiceNumber,
        InvoiceDate = invoice.InvoiceDate,
        DueDate = invoice.DueDate,
        PaymentTerms = invoice.PaymentTerms,
        SubtotalAmount = invoice.SubtotalAmount,
        DiscountAmount = invoice.DiscountAmount,
        TaxAmount = invoice.TaxAmount,
        TotalAmount = invoice.TotalAmount,
        AmountPaid = invoice.AmountPaid,
        OutstandingBalance = invoice.OutstandingBalance,
        PaymentStatus = invoice.PaymentStatus,
        Notes = invoice.Notes,
    };
}
