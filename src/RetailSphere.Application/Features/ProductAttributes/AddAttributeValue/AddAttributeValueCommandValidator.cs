using FluentValidation;

namespace RetailSphere.Application.Features.ProductAttributes.AddAttributeValue;

public sealed class AddAttributeValueCommandValidator : AbstractValidator<AddAttributeValueCommand>
{
    public AddAttributeValueCommandValidator()
    {
        RuleFor(x => x.Value).NotEmpty().MaximumLength(100);
    }
}
