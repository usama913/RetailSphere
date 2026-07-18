using FluentValidation;

namespace RetailSphere.Application.Features.ProductAttributes.CreateProductAttribute;

public sealed class CreateProductAttributeCommandValidator : AbstractValidator<CreateProductAttributeCommand>
{
    public CreateProductAttributeCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
