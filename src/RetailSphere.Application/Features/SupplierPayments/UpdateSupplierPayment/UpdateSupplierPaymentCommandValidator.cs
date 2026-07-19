using FluentValidation;

namespace RetailSphere.Application.Features.SupplierPayments.UpdateSupplierPayment;

public sealed class UpdateSupplierPaymentCommandValidator : AbstractValidator<UpdateSupplierPaymentCommand>
{
    public UpdateSupplierPaymentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.ReferenceNumber).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
