using FluentValidation;

namespace RetailSphere.Application.Features.ProductAttributes.UpdateProductAttribute;

public sealed class UpdateProductAttributeCommandValidator : AbstractValidator<UpdateProductAttributeCommand>
{
    public UpdateProductAttributeCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
