using FluentValidation;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.CreateStockTransfer;

public sealed class CreateStockTransferCommandValidator : AbstractValidator<CreateStockTransferCommand>
{
    public CreateStockTransferCommandValidator()
    {
        RuleFor(x => x.FromBranchId).GreaterThan(0);
        RuleFor(x => x.ToBranchId).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}
