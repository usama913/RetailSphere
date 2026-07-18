namespace RetailSphere.Contracts.Customers;

public sealed class CustomerDto
{
    public required long Id { get; init; }

    public required string Name { get; init; }

    public string? Phone { get; init; }

    public string? Email { get; init; }

    public string? Address { get; init; }

    public required bool IsActive { get; init; }
}

public sealed class CreateCustomerRequest
{
    public required string Name { get; init; }

    public string? Phone { get; init; }

    public string? Email { get; init; }

    public string? Address { get; init; }
}

public sealed class UpdateCustomerRequest
{
    public required string Name { get; init; }

    public string? Phone { get; init; }

    public string? Email { get; init; }

    public string? Address { get; init; }
}
