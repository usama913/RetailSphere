using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.IdentityAccess.ValueObjects;

/// <summary>
/// Wraps an already-hashed password. The Domain layer never hashes passwords
/// itself (no hashing algorithm dependency in Domain) — hashing happens in
/// Infrastructure (IPasswordHasher), and only the resulting hash is handed
/// to this value object for storage on the User aggregate.
/// </summary>
public sealed class HashedPassword : ValueObject
{
    public string Value { get; }

    private HashedPassword(string value)
    {
        Value = value;
    }

    public static Result<HashedPassword> FromHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            return Result.Failure<HashedPassword>(Error.Validation("Password.EmptyHash", "Password hash cannot be empty."));

        return Result.Success(new HashedPassword(hash));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
