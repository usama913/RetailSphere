using FluentValidation;

namespace RetailSphere.Application.Features.CustomerPayments.ReverseCustomerPayment;

public sealed class ReverseCustomerPaymentCommandValidator : AbstractValidator<ReverseCustomerPaymentCommand>
{
    public ReverseCustomerPaymentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
