using FluentValidation;

namespace RetailSphere.Application.Features.CustomerPayments.UpdateCustomerPayment;

public sealed class UpdateCustomerPaymentCommandValidator : AbstractValidator<UpdateCustomerPaymentCommand>
{
    public UpdateCustomerPaymentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.ReferenceNumber).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
