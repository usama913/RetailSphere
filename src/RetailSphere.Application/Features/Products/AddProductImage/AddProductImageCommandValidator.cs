using FluentValidation;

namespace RetailSphere.Application.Features.Products.AddProductImage;

public sealed class AddProductImageCommandValidator : AbstractValidator<AddProductImageCommand>
{
    public AddProductImageCommandValidator()
    {
        RuleFor(x => x.Url).NotEmpty().MaximumLength(1000);
    }
}
