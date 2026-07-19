using FluentValidation;

namespace RetailSphere.Application.Features.CustomerPayments.RecordCustomerPayment;

public sealed class RecordCustomerPaymentCommandValidator : AbstractValidator<RecordCustomerPaymentCommand>
{
    public RecordCustomerPaymentCommandValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.ReferenceNumber).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(1000);
        RuleForEach(x => x.Allocations).ChildRules(allocation =>
        {
            allocation.RuleFor(a => a.SalesOrderId).GreaterThan(0);
            allocation.RuleFor(a => a.Amount).GreaterThan(0);
        });
    }
}
