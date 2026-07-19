using FluentValidation;

namespace RetailSphere.Application.Features.SalesReturns.CreateSalesReturn;

public sealed class CreateSalesReturnCommandValidator : AbstractValidator<CreateSalesReturnCommand>
{
    public CreateSalesReturnCommandValidator()
    {
        RuleFor(x => x.SalesOrderId).GreaterThan(0);
        RuleFor(x => x.Reason).MaximumLength(1000);
        RuleFor(x => x.Lines).NotEmpty().WithMessage("Select at least one item to return.");

        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.SalesOrderLineId).GreaterThan(0);
            line.RuleFor(l => l.Quantity).GreaterThan(0);
        });
    }
}
