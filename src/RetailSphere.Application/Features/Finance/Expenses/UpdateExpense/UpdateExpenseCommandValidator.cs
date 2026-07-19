using FluentValidation;

namespace RetailSphere.Application.Features.Finance.Expenses.UpdateExpense;

public sealed class UpdateExpenseCommandValidator : AbstractValidator<UpdateExpenseCommand>
{
    public UpdateExpenseCommandValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Category).MaximumLength(50);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}
