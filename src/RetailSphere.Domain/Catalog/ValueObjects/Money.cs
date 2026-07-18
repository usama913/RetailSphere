using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Catalog.ValueObjects;

/// <summary>
/// A non-negative monetary amount in a specific currency. Catalog prices are
/// branch-agnostic for now (per the Pakistan-first launch decision — see the
/// architecture doc's Decisions Log), so this defaults to PKR; the multi-region
/// tax/currency model can attach branch-specific pricing later without
/// reshaping this value object.
/// </summary>
public sealed class Money : ValueObject
{
    public decimal Amount { get; }

    public string CurrencyCode { get; }

    private Money(decimal amount, string currencyCode)
    {
        Amount = amount;
        CurrencyCode = currencyCode;
    }

    public static Result<Money> Create(decimal amount, string currencyCode = "PKR")
    {
        if (amount < 0)
            return Result.Failure<Money>(Error.Validation("Money.NegativeAmount", "Amount cannot be negative."));

        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Trim().Length != 3)
            return Result.Failure<Money>(Error.Validation("Money.InvalidCurrencyCode", "Currency code must be a 3-letter ISO 4217 code."));

        return Result.Success(new Money(amount, currencyCode.Trim().ToUpperInvariant()));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return CurrencyCode;
    }

    public override string ToString() => $"{Amount:0.00} {CurrencyCode}";
}
