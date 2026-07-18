using FluentValidation;

namespace RetailSphere.Application.Features.PurchaseOrders.AddPurchaseOrderLine;

public sealed class AddPurchaseOrderLineCommandValidator : AbstractValidator<AddPurchaseOrderLineCommand>
{
    public AddPurchaseOrderLineCommandValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.ProductVariantId).GreaterThan(0);
        RuleFor(x => x.QuantityOrdered).GreaterThan(0);
        RuleFor(x => x.UnitCost).GreaterThanOrEqualTo(0);
    }
}
