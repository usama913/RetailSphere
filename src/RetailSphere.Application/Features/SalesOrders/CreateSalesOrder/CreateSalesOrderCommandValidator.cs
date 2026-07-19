using FluentValidation;

namespace RetailSphere.Application.Features.SalesOrders.CreateSalesOrder;

public sealed class CreateSalesOrderCommandValidator : AbstractValidator<CreateSalesOrderCommand>
{
    public CreateSalesOrderCommandValidator()
    {
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.OrderDiscountAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.AmountPaid).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Notes).MaximumLength(2000);
        RuleFor(x => x.PaymentTerms).MaximumLength(30);
        RuleFor(x => x.Lines).NotEmpty().WithMessage("Add at least one item before checking out.");

        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.ProductId).GreaterThan(0);
            line.RuleFor(l => l.ProductVariantId).GreaterThan(0);
            line.RuleFor(l => l.Quantity).GreaterThan(0);
            line.RuleFor(l => l.DiscountAmount).GreaterThanOrEqualTo(0);
        });
    }
}
