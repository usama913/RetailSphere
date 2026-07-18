namespace RetailSphere.Domain.IdentityAccess;

/// <summary>
/// One repository per aggregate root — returns/persists the whole User aggregate
/// (including its RefreshTokens) so invariants are never violated by partial loads.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<User?> GetByRefreshTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Paged, optionally filtered listing for the Admin &gt; Users screen (Phase 1).</summary>
    Task<(IReadOnlyList<User> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        string? search,
        long? branchId,
        bool? isActive,
        CancellationToken cancellationToken = default);

    void Add(User user);

    void Update(User user);
}
