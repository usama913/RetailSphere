using FluentValidation;

namespace RetailSphere.Application.Features.Finance.Expenses.CreateExpense;

public sealed class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand>
{
    public CreateExpenseCommandValidator()
    {
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Category).MaximumLength(50);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}
