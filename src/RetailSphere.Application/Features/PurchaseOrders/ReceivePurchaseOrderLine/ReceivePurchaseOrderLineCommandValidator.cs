using FluentValidation;

namespace RetailSphere.Application.Features.PurchaseOrders.ReceivePurchaseOrderLine;

public sealed class ReceivePurchaseOrderLineCommandValidator : AbstractValidator<ReceivePurchaseOrderLineCommand>
{
    public ReceivePurchaseOrderLineCommandValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
