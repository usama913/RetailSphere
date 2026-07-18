namespace RetailSphere.Contracts.Admin;

public sealed class BranchDto
{
    public required long Id { get; init; }

    public required string Name { get; init; }

    public required string Code { get; init; }

    public string? Address { get; init; }

    public string? City { get; init; }

    public required string CurrencyCode { get; init; }

    public required bool IsActive { get; init; }
}

public sealed class CreateBranchRequest
{
    public required string Name { get; init; }

    public required string Code { get; init; }

    public string? Address { get; init; }

    public string? City { get; init; }

    public string CurrencyCode { get; init; } = "PKR";
}

public sealed class UpdateBranchRequest
{
    public required string Name { get; init; }

    public string? Address { get; init; }

    public string? City { get; init; }

    public required string CurrencyCode { get; init; }
}
