namespace RetailSphere.Contracts.Catalog;

public sealed class CategoryDto
{
    public required long Id { get; init; }

    public required string Name { get; init; }

    public required string Slug { get; init; }

    public long? ParentCategoryId { get; init; }

    public required bool IsActive { get; init; }
}

public sealed class CreateCategoryRequest
{
    public required string Name { get; init; }

    public long? ParentCategoryId { get; init; }
}

public sealed class UpdateCategoryRequest
{
    public required string Name { get; init; }

    public long? ParentCategoryId { get; init; }
}
