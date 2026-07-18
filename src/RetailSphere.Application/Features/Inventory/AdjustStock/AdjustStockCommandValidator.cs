using FluentValidation;

namespace RetailSphere.Application.Features.Inventory.AdjustStock;

public sealed class AdjustStockCommandValidator : AbstractValidator<AdjustStockCommand>
{
    public AdjustStockCommandValidator()
    {
        RuleFor(x => x.ProductVariantId).GreaterThan(0);
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.QuantityDelta).NotEqual(0).WithMessage("Quantity change must not be zero.");
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
