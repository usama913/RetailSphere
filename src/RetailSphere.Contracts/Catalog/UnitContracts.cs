namespace RetailSphere.Contracts.Catalog;

public sealed class UnitDto
{
    public required long Id { get; init; }

    public required string Name { get; init; }

    public required string ShortCode { get; init; }

    public required bool AllowDecimal { get; init; }

    public required bool IsActive { get; init; }
}

public sealed class CreateUnitRequest
{
    public required string Name { get; init; }

    public required string ShortCode { get; init; }

    public bool AllowDecimal { get; init; } = true;
}

public sealed class UpdateUnitRequest
{
    public required string Name { get; init; }

    public required string ShortCode { get; init; }

    public bool AllowDecimal { get; init; } = true;
}
