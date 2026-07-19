using FluentValidation;

namespace RetailSphere.Application.Features.SupplierPayments.ReverseSupplierPayment;

public sealed class ReverseSupplierPaymentCommandValidator : AbstractValidator<ReverseSupplierPaymentCommand>
{
    public ReverseSupplierPaymentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
