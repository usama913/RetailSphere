using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.IdentityAccess.Events;

public sealed record UserRegisteredEvent(long UserId, string Email) : IDomainEvent;

public sealed record UserLoggedInEvent(long UserId, DateTimeOffset OccurredOnUtc) : IDomainEvent;

public sealed record UserPasswordChangedEvent(long UserId) : IDomainEvent;

public sealed record UserDeactivatedEvent(long UserId) : IDomainEvent;

public sealed record UserRoleAssignedEvent(long UserId, long RoleId) : IDomainEvent;

public sealed record UserRoleRevokedEvent(long UserId, long RoleId) : IDomainEvent;

public sealed record RefreshTokenIssuedEvent(long UserId, long RefreshTokenId) : IDomainEvent;

public sealed record RefreshTokenRevokedEvent(long UserId, long RefreshTokenId, string Reason) : IDomainEvent;
