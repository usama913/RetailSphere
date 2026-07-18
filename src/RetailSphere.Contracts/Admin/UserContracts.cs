namespace RetailSphere.Contracts.Admin;

public sealed class UserDto
{
    public required long Id { get; init; }

    public required string Email { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public long? BranchId { get; init; }

    public string? BranchName { get; init; }

    public required bool IsActive { get; init; }

    public DateTime? LastLoginAtUtc { get; init; }

    public required IReadOnlyList<long> RoleIds { get; init; }

    public required IReadOnlyList<string> RoleNames { get; init; }
}

public sealed class CreateUserRequest
{
    public required string Email { get; init; }

    public required string Password { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public long? BranchId { get; init; }

    public IReadOnlyList<long> RoleIds { get; init; } = [];
}

public sealed class UpdateUserRequest
{
    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public long? BranchId { get; init; }
}

public sealed class AssignRolesRequest
{
    public required IReadOnlyList<long> RoleIds { get; init; }
}

public sealed class ResetPasswordRequest
{
    public required string NewPassword { get; init; }
}
