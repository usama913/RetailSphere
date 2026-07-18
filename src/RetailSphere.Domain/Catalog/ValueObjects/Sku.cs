using System.Text.RegularExpressions;
using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Catalog.ValueObjects;

/// <summary>
/// The unique code identifying one sellable unit (a Product Variant — never a
/// bare Product, per the architecture doc's core catalog decision). Normalized
/// to upper invariant so lookups (e.g. by a POS barcode scanner falling back to
/// manual SKU entry) never miss on casing.
/// </summary>
public sealed partial class Sku : ValueObject
{
    public string Value { get; }

    private Sku(string value)
    {
        Value = value;
    }

    public static Result<Sku> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Sku>(Error.Validation("Sku.Empty", "SKU cannot be empty."));

        value = value.Trim().ToUpperInvariant();

        if (value.Length > 64)
            return Result.Failure<Sku>(Error.Validation("Sku.TooLong", "SKU cannot exceed 64 characters."));

        if (!SkuRegex().IsMatch(value))
            return Result.Failure<Sku>(Error.Validation("Sku.InvalidFormat", "SKU may only contain letters, numbers, and hyphens."));

        return Result.Success(new Sku(value));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[A-Z0-9\-]+$")]
    private static partial Regex SkuRegex();
}
