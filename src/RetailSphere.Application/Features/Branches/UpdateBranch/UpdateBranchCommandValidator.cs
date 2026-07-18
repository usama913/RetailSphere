using FluentValidation;

namespace RetailSphere.Application.Features.Branches.UpdateBranch;

public sealed class UpdateBranchCommandValidator : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);
    }
}
