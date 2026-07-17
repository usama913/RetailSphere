using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Options;

namespace RetailSphere.Infrastructure.Security;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly AuthOptions _authOptions;
    private readonly RSA _privateKey;

    public JwtTokenService(IOptions<JwtOptions> jwtOptions, IOptions<AuthOptions> authOptions)
    {
        _jwtOptions = jwtOptions.Value;
        _authOptions = authOptions.Value;

        _privateKey = RSA.Create();
        _privateKey.ImportFromPem(File.ReadAllText(_jwtOptions.PrivateKeyPath));
    }

    public (string AccessToken, DateTime ExpiresAtUtc) GenerateAccessToken(
        long userId,
        string email,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissions,
        long? branchId)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_authOptions.AccessTokenLifetimeMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if (branchId.HasValue)
            claims.Add(new Claim("branch_id", branchId.Value.ToString()));

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(permissions.Select(permission => new Claim("permission", permission)));

        var signingCredentials = new SigningCredentials(new RsaSecurityKey(_privateKey), SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc,
            signingCredentials: signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        return (accessToken, expiresAtUtc);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }

    public string HashRefreshToken(string rawRefreshToken)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(rawRefreshToken);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
