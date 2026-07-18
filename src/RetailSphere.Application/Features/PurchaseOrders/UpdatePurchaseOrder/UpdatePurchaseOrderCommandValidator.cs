using FluentValidation;

namespace RetailSphere.Application.Features.PurchaseOrders.UpdatePurchaseOrder;

public sealed class UpdatePurchaseOrderCommandValidator : AbstractValidator<UpdatePurchaseOrderCommand>
{
    public UpdatePurchaseOrderCommandValidator()
    {
        RuleFor(x => x.SupplierId).GreaterThan(0);
        RuleFor(x => x.BranchId).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}
