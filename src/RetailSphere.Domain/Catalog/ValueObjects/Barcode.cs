using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Catalog.ValueObjects;

/// <summary>
/// A scannable barcode (EAN-13/UPC-A/etc.) manually entered against a variant.
/// v1 deliberately does not generate or validate barcode check digits against a
/// specific symbology — most retailers already receive barcodes from suppliers;
/// generating compliant new ones is a separate concern to revisit if RetailSphere
/// ever needs to mint its own (e.g. for private-label goods).
/// </summary>
public sealed class Barcode : ValueObject
{
    public string Value { get; }

    private Barcode(string value)
    {
        Value = value;
    }

    public static Result<Barcode> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Barcode>(Error.Validation("Barcode.Empty", "Barcode cannot be empty."));

        value = value.Trim();

        if (value.Length > 64)
            return Result.Failure<Barcode>(Error.Validation("Barcode.TooLong", "Barcode cannot exceed 64 characters."));

        return Result.Success(new Barcode(value));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
