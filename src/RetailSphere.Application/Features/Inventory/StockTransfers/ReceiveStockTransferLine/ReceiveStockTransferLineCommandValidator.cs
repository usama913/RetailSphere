using FluentValidation;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.ReceiveStockTransferLine;

public sealed class ReceiveStockTransferLineCommandValidator : AbstractValidator<ReceiveStockTransferLineCommand>
{
    public ReceiveStockTransferLineCommandValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
