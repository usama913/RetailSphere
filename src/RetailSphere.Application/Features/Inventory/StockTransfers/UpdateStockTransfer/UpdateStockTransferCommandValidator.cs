using FluentValidation;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.UpdateStockTransfer;

public sealed class UpdateStockTransferCommandValidator : AbstractValidator<UpdateStockTransferCommand>
{
    public UpdateStockTransferCommandValidator()
    {
        RuleFor(x => x.FromBranchId).GreaterThan(0);
        RuleFor(x => x.ToBranchId).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}
