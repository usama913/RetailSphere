using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;

namespace RetailSphere.Application.Features.SupplierPayments;

internal static class SupplierPaymentMappings
{
    public static SupplierPaymentDto ToDto(
        SupplierPayment payment,
        string? supplierName,
        string? purchaseInvoiceSupplierInvoiceNumber,
        string? branchName,
        string? reversedByUserName) => new()
    {
        Id = payment.Id,
        SupplierId = payment.SupplierId,
        SupplierName = supplierName,
        PurchaseInvoiceId = payment.PurchaseInvoiceId,
        PurchaseInvoiceSupplierInvoiceNumber = purchaseInvoiceSupplierInvoiceNumber,
        BranchId = payment.BranchId,
        BranchName = branchName,
        PaymentDate = payment.PaymentDate,
        Amount = payment.Amount,
        PaymentMethod = payment.PaymentMethod,
        ReferenceNumber = payment.ReferenceNumber,
        Notes = payment.Notes,
        IsReversed = payment.IsReversed,
        ReversalReason = payment.ReversalReason,
        ReversedAtUtc = payment.ReversedAtUtc,
        ReversedByUserName = reversedByUserName,
    };
}
