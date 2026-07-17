using RetailSphere.Domain.IdentityAccess.Events;
using RetailSphere.Domain.IdentityAccess.ValueObjects;
using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.IdentityAccess;

/// <summary>
/// The Identity &amp; Access aggregate root. Owns its role assignments and refresh
/// tokens; nothing outside this aggregate mutates them directly.
/// </summary>
public sealed class User : AggregateRoot<long>, IAuditableEntity, ISoftDeletable
{
    private readonly List<long> _roleIds = [];
    private readonly List<RefreshToken> _refreshTokens = [];

    public Email Email { get; private set; } = default!;

    /// <summary>
    /// Plain-string mirror of Email.Value, kept only so lookups (GetByEmailAsync,
    /// EmailExistsAsync) can filter on an ordinary mapped string column. EF Core
    /// can't translate a query that reaches through a value-converted property's
    /// members (e.g. `u.Email.Value == x`) into SQL — comparing this plain column
    /// instead sidesteps that limitation entirely. Same idea as ASP.NET Core
    /// Identity's own NormalizedEmail/NormalizedUserName columns.
    /// </summary>
    public string NormalizedEmail { get; private set; } = default!;

    public HashedPassword Password { get; private set; } = default!;

    public string FirstName { get; private set; } = default!;

    public string LastName { get; private set; } = default!;

    /// <summary>Home branch. Null for users (e.g. Super Admin) whose access spans all branches.</summary>
    public long? BranchId { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime? LastLoginAtUtc { get; private set; }

    public IReadOnlyCollection<long> RoleIds => _roleIds.AsReadOnly();

    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public long? DeletedBy { get; set; }

    private User()
    {
    }

    public static Result<User> Register(Email email, HashedPassword password, string firstName, string lastName, long? branchId)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure<User>(Error.Validation("User.FirstNameRequired", "First name is required."));

        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure<User>(Error.Validation("User.LastNameRequired", "Last name is required."));

        var user = new User
        {
            Email = email,
            NormalizedEmail = email.Value,
            Password = password,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            BranchId = branchId,
            IsActive = true,
        };

        user.Raise(new UserRegisteredEvent(user.Id, email.Value));
        return Result.Success(user);
    }

    public void ChangePassword(HashedPassword newPassword)
    {
        Password = newPassword;
        Raise(new UserPasswordChangedEvent(Id));
    }

    public Result AssignRole(long roleId)
    {
        if (_roleIds.Contains(roleId))
            return Result.Failure(Error.Conflict("User.RoleAlreadyAssigned", "The user already has this role."));

        _roleIds.Add(roleId);
        Raise(new UserRoleAssignedEvent(Id, roleId));
        return Result.Success();
    }

    public Result RevokeRole(long roleId)
    {
        if (!_roleIds.Remove(roleId))
            return Result.Failure(Error.NotFound("User.RoleNotAssigned", "The user does not have this role."));

        Raise(new UserRoleRevokedEvent(Id, roleId));
        return Result.Success();
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        Raise(new UserDeactivatedEvent(Id));
    }

    public void Activate() => IsActive = true;

    public void RecordLogin()
    {
        LastLoginAtUtc = DateTime.UtcNow;
        Raise(new UserLoggedInEvent(Id, DateTimeOffset.UtcNow));
    }

    /// <summary>
    /// Issues a new refresh token and returns it (the caller — an application
    /// service — is responsible for generating the raw token and passing only its
    /// hash here; the raw value is never stored).
    /// </summary>
    public RefreshToken IssueRefreshToken(string tokenHash, DateTime expiresAtUtc, string? createdByIp)
    {
        var token = RefreshToken.Issue(Id, tokenHash, expiresAtUtc, createdByIp);
        _refreshTokens.Add(token);
        Raise(new RefreshTokenIssuedEvent(Id, token.Id));
        return token;
    }

    /// <summary>
    /// Rotates a refresh token: revokes the old one, links it to the new one.
    /// </summary>
    public RefreshToken RotateRefreshToken(RefreshToken current, string newTokenHash, DateTime newExpiresAtUtc, string? createdByIp)
    {
        var replacement = IssueRefreshToken(newTokenHash, newExpiresAtUtc, createdByIp);
        current.Revoke(replacement.Id);
        return replacement;
    }

    /// <summary>
    /// Reuse-detection response: if a token that was already rotated is presented
    /// again, the entire family must be revoked and the user forced to re-authenticate.
    /// </summary>
    public void RevokeRefreshTokenFamily(RefreshToken compromisedToken, string reason)
    {
        compromisedToken.Revoke();
        foreach (var token in _refreshTokens.Where(t => t.IsActive))
        {
            token.Revoke();
        }

        Raise(new RefreshTokenRevokedEvent(Id, compromisedToken.Id, reason));
    }
}
