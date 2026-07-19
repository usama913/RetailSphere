using FluentValidation;

namespace RetailSphere.Application.Features.Finance.CashRegister.CloseCashRegisterSession;

public sealed class CloseCashRegisterSessionCommandValidator : AbstractValidator<CloseCashRegisterSessionCommand>
{
    public CloseCashRegisterSessionCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.ClosingBalance).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ClosingNotes).MaximumLength(1000);
    }
}
