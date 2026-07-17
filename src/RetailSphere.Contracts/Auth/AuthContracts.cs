namespace RetailSphere.Contracts.Auth;

public sealed class LoginRequest
{
    public required string Email { get; init; }

    public required string Password { get; init; }
}

public sealed class LoginResponse
{
    public required string AccessToken { get; init; }

    public required DateTime AccessTokenExpiresAtUtc { get; init; }

    public required string RefreshToken { get; init; }

    public required long UserId { get; init; }

    public required string FullName { get; init; }

    public long? BranchId { get; init; }

    public required IReadOnlyList<string> Roles { get; init; }

    public required IReadOnlyList<string> Permissions { get; init; }
}

public sealed class RefreshTokenRequest
{
    public required string RefreshToken { get; init; }
}

public sealed class LogoutRequest
{
    public required string RefreshToken { get; init; }
}
