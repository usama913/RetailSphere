using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseInvoices.UpdatePurchaseInvoice;

public sealed record UpdatePurchaseInvoiceCommand(
    long Id,
    string SupplierInvoiceNumber,
    DateTime InvoiceDate,
    DateTime DueDate,
    string? PaymentTerms,
    decimal SubtotalAmount,
    decimal DiscountAmount,
    decimal TaxAmount,
    string? Notes) : IRequest<Result<PurchaseInvoiceDto>>;
