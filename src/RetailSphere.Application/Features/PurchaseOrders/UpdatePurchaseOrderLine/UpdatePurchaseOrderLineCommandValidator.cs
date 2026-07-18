using FluentValidation;

namespace RetailSphere.Application.Features.PurchaseOrders.UpdatePurchaseOrderLine;

public sealed class UpdatePurchaseOrderLineCommandValidator : AbstractValidator<UpdatePurchaseOrderLineCommand>
{
    public UpdatePurchaseOrderLineCommandValidator()
    {
        RuleFor(x => x.QuantityOrdered).GreaterThan(0);
        RuleFor(x => x.UnitCost).GreaterThanOrEqualTo(0);
    }
}
