using FluentValidation;

namespace RetailSphere.Application.Features.Units.CreateUnit;

public sealed class CreateUnitCommandValidator : AbstractValidator<CreateUnitCommand>
{
    public CreateUnitCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ShortCode).NotEmpty().MaximumLength(20);
    }
}
