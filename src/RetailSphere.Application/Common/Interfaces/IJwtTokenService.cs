namespace RetailSphere.Application.Common.Interfaces;

/// <summary>
/// Implemented in Infrastructure. Access tokens are short-lived JWTs (RS256);
/// refresh tokens are opaque random strings — only their hash is ever persisted,
/// and this service is what computes that hash consistently for lookups too.
/// </summary>
public interface IJwtTokenService
{
    (string AccessToken, DateTime ExpiresAtUtc) GenerateAccessToken(
        long userId,
        string email,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissions,
        long? branchId);

    string GenerateRefreshToken();

    string HashRefreshToken(string rawRefreshToken);
}
