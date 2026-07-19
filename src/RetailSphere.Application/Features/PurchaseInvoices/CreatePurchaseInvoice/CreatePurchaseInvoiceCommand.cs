using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseInvoices.CreatePurchaseInvoice;

public sealed record CreatePurchaseInvoiceCommand(
    long SupplierId,
    long BranchId,
    long? PurchaseOrderId,
    string SupplierInvoiceNumber,
    DateTime InvoiceDate,
    DateTime DueDate,
    string? PaymentTerms,
    decimal SubtotalAmount,
    decimal DiscountAmount,
    decimal TaxAmount,
    string? Notes) : IRequest<Result<PurchaseInvoiceDto>>;
