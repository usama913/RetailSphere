using FluentValidation;

namespace RetailSphere.Application.Features.Inventory.TransferStock;

public sealed class TransferStockCommandValidator : AbstractValidator<TransferStockCommand>
{
    public TransferStockCommandValidator()
    {
        RuleFor(x => x.ProductVariantId).GreaterThan(0);
        RuleFor(x => x.FromBranchId).GreaterThan(0);
        RuleFor(x => x.ToBranchId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
        RuleFor(x => x)
            .Must(x => x.FromBranchId != x.ToBranchId)
            .WithMessage("Source and destination branches must be different.");
    }
}
