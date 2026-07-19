using FluentValidation;

namespace RetailSphere.Application.Features.PurchaseInvoices.CreatePurchaseInvoice;

public sealed class CreatePurchaseInvoiceCommandValidator : AbstractValidator<CreatePurchaseInvoiceCommand>
{
    public CreatePurchaseInvoiceCommandValidator()
    {
        RuleFor(x => x.SupplierId).GreaterThan(0);
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.SupplierInvoiceNumber).NotEmpty().MaximumLength(60);
        RuleFor(x => x.PaymentTerms).MaximumLength(30);
        RuleFor(x => x.SubtotalAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DiscountAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TaxAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Notes).MaximumLength(2000);
        RuleFor(x => x.DueDate).GreaterThanOrEqualTo(x => x.InvoiceDate);
    }
}
