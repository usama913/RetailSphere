using FluentValidation;

namespace RetailSphere.Application.Features.Customers.UpdateCustomerCreditLimit;

public sealed class UpdateCustomerCreditLimitCommandValidator : AbstractValidator<UpdateCustomerCreditLimitCommand>
{
    public UpdateCustomerCreditLimitCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CreditLimit).GreaterThanOrEqualTo(0).When(x => x.CreditLimit.HasValue);
    }
}
