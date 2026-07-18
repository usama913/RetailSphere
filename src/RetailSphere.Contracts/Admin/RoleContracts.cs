namespace RetailSphere.Contracts.Admin;

public sealed class PermissionDto
{
    public required long Id { get; init; }

    public required string Code { get; init; }

    public required string DisplayName { get; init; }

    public required string Module { get; init; }
}

public sealed class RoleDto
{
    public required long Id { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public required bool IsSystemRole { get; init; }

    public required IReadOnlyList<long> PermissionIds { get; init; }
}

public sealed class CreateRoleRequest
{
    public required string Name { get; init; }

    public string? Description { get; init; }

    public IReadOnlyList<long> PermissionIds { get; init; } = [];
}

public sealed class UpdateRoleRequest
{
    public required string Name { get; init; }

    public string? Description { get; init; }

    public IReadOnlyList<long> PermissionIds { get; init; } = [];
}
