using FluentValidation;

namespace RetailSphere.Application.Features.Suppliers.CreateSupplier;

public sealed class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ContactPerson).MaximumLength(150);
        RuleFor(x => x.Email).MaximumLength(200).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Phone).MaximumLength(50);
        RuleFor(x => x.Address).MaximumLength(500);
        RuleFor(x => x.TaxNumber).MaximumLength(50);
    }
}
