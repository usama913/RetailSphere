using FluentValidation;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.AddStockTransferLine;

public sealed class AddStockTransferLineCommandValidator : AbstractValidator<AddStockTransferLineCommand>
{
    public AddStockTransferLineCommandValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.ProductVariantId).GreaterThan(0);
        RuleFor(x => x.QuantityRequested).GreaterThan(0);
    }
}
