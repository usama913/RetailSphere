using FluentValidation;
using RetailSphere.Domain.Catalog;

namespace RetailSphere.Application.Features.Products.AddVariant;

public sealed class AddVariantCommandValidator : AbstractValidator<AddVariantCommand>
{
    public AddVariantCommandValidator()
    {
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CompareAtPrice).GreaterThanOrEqualTo(0).When(x => x.CompareAtPrice.HasValue);
        RuleFor(x => x.CostPrice).GreaterThanOrEqualTo(0).When(x => x.CostPrice.HasValue);
        RuleFor(x => x.BarcodeType).Must(t => ProductVariant.BarcodeTypes.Contains(t!))
            .When(x => !string.IsNullOrWhiteSpace(x.BarcodeType))
            .WithMessage("Unrecognized barcode type.");
        RuleFor(x => x.TaxRate).InclusiveBetween(0, 100);
        RuleFor(x => x.TaxType).Must(t => ProductVariant.TaxTypes.Contains(t!))
            .When(x => !string.IsNullOrWhiteSpace(x.TaxType))
            .WithMessage("Tax type must be 'Exclusive' or 'Inclusive'.");
        RuleFor(x => x.Weight).GreaterThanOrEqualTo(0).When(x => x.Weight.HasValue);
        RuleFor(x => x.Length).GreaterThanOrEqualTo(0).When(x => x.Length.HasValue);
        RuleFor(x => x.Width).GreaterThanOrEqualTo(0).When(x => x.Width.HasValue);
        RuleFor(x => x.Height).GreaterThanOrEqualTo(0).When(x => x.Height.HasValue);
        RuleFor(x => x.ReorderPoint).GreaterThanOrEqualTo(0).When(x => x.ReorderPoint.HasValue);
    }
}
