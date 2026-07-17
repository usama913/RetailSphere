using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.IdentityAccess;

namespace RetailSphere.Persistence.Repositories;

public sealed class UserRepository(RetailSphereDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = Normalize(email);
        return dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalized, cancellationToken);
    }

    public Task<User?> GetByRefreshTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.TokenHash == tokenHash), cancellationToken);

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = Normalize(email);
        return dbContext.Users.AnyAsync(u => u.NormalizedEmail == normalized, cancellationToken);
    }

    // Mirrors Email.Create's own normalization (trim + invariant lowercase) so a
    // lookup always matches however the value was normalized when it was stored.
    private static string Normalize(string email) => email.Trim().ToLowerInvariant();

    public void Add(User user) => dbContext.Users.Add(user);

    public void Update(User user) => dbContext.Users.Update(user);
}
