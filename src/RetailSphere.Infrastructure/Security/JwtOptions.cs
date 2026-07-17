namespace RetailSphere.Infrastructure.Security;

/// <summary>
/// Bound from configuration ("Jwt" section). RS256 (asymmetric) per §7 of the
/// architecture doc — the API holds the private key and signs; any future service
/// that only needs to validate tokens can be handed just the public key.
///
/// For local development, generate a dev keypair with:
///   openssl genrsa -out jwt-private.pem 2048
///   openssl rsa -in jwt-private.pem -pubout -out jwt-public.pem
/// Never commit real keys — these paths should point at secret-mounted files in
/// every environment beyond a developer's own machine.
/// </summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public required string Issuer { get; init; }

    public required string Audience { get; init; }

    public required string PrivateKeyPath { get; init; }

    public required string PublicKeyPath { get; init; }
}
