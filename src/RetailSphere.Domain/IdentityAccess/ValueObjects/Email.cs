using System.Text.RegularExpressions;
using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.IdentityAccess.ValueObjects;

public sealed partial class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Email>(Error.Validation("Email.Empty", "Email cannot be empty."));

        value = value.Trim();

        if (value.Length > 256)
            return Result.Failure<Email>(Error.Validation("Email.TooLong", "Email cannot exceed 256 characters."));

        if (!EmailRegex().IsMatch(value))
            return Result.Failure<Email>(Error.Validation("Email.InvalidFormat", "Email is not a valid address."));

        return Result.Success(new Email(value.ToLowerInvariant()));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
