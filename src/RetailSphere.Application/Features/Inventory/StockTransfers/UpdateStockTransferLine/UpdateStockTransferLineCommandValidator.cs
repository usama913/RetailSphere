using FluentValidation;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.UpdateStockTransferLine;

public sealed class UpdateStockTransferLineCommandValidator : AbstractValidator<UpdateStockTransferLineCommand>
{
    public UpdateStockTransferLineCommandValidator()
    {
        RuleFor(x => x.QuantityRequested).GreaterThan(0);
    }
}
