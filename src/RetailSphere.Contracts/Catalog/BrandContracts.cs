namespace RetailSphere.Contracts.Catalog;

public sealed class BrandDto
{
    public required long Id { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public required bool IsActive { get; init; }
}

public sealed class CreateBrandRequest
{
    public required string Name { get; init; }

    public string? Description { get; init; }
}

public sealed class UpdateBrandRequest
{
    public required string Name { get; init; }

    public string? Description { get; init; }
}
