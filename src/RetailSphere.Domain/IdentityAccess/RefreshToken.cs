using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.IdentityAccess;

/// <summary>
/// A rotated, single-use refresh token belonging to a <see cref="User"/>.
/// Only the hash is ever persisted — the raw token is handed to the client once,
/// at issuance, and is never recoverable from storage. Rotation + reuse detection:
/// using an already-rotated token revokes the entire token family (see
/// <see cref="User.RevokeRefreshTokenFamily"/>).
/// </summary>
public sealed class RefreshToken : Entity<long>
{
    public long UserId { get; private set; }

    public string TokenHash { get; private set; } = default!;

    public DateTime ExpiresAtUtc { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public string? CreatedByIp { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    public long? ReplacedByTokenId { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

    public bool IsRevoked => RevokedAtUtc is not null;

    public bool IsActive => !IsExpired && !IsRevoked;

    private RefreshToken()
    {
    }

    internal static RefreshToken Issue(long userId, string tokenHash, DateTime expiresAtUtc, string? createdByIp) => new()
    {
        UserId = userId,
        TokenHash = tokenHash,
        ExpiresAtUtc = expiresAtUtc,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedByIp = createdByIp,
    };

    internal void Revoke(long? replacedByTokenId = null)
    {
        RevokedAtUtc = DateTime.UtcNow;
        ReplacedByTokenId = replacedByTokenId;
    }
}
