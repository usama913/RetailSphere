using FluentValidation;

namespace RetailSphere.Application.Features.Finance.CashRegister.OpenCashRegisterSession;

public sealed class OpenCashRegisterSessionCommandValidator : AbstractValidator<OpenCashRegisterSessionCommand>
{
    public OpenCashRegisterSessionCommandValidator()
    {
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.OpeningBalance).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OpeningNotes).MaximumLength(1000);
    }
}
