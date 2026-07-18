using FluentValidation;

namespace RetailSphere.Application.Features.Units.UpdateUnit;

public sealed class UpdateUnitCommandValidator : AbstractValidator<UpdateUnitCommand>
{
    public UpdateUnitCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ShortCode).NotEmpty().MaximumLength(20);
    }
}
