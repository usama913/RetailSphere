using FluentValidation;

namespace RetailSphere.Application.Features.CustomerPayments.AllocateCustomerPayment;

public sealed class AllocateCustomerPaymentCommandValidator : AbstractValidator<AllocateCustomerPaymentCommand>
{
    public AllocateCustomerPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId).GreaterThan(0);
        RuleFor(x => x.SalesOrderId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
