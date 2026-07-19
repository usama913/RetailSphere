using FluentValidation;

namespace RetailSphere.Application.Features.Suppliers.UpdateSupplierCreditTerms;

public sealed class UpdateSupplierCreditTermsCommandValidator : AbstractValidator<UpdateSupplierCreditTermsCommand>
{
    public UpdateSupplierCreditTermsCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CreditLimit).GreaterThanOrEqualTo(0).When(x => x.CreditLimit.HasValue);
        RuleFor(x => x.PaymentTerms).MaximumLength(30);
    }
}
