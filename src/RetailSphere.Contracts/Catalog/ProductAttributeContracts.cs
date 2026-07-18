namespace RetailSphere.Contracts.Catalog;

public sealed class AttributeValueDto
{
    public required long Id { get; init; }

    public required string Value { get; init; }

    public required int DisplayOrder { get; init; }
}

public sealed class ProductAttributeDto
{
    public required long Id { get; init; }

    public required string Name { get; init; }

    public required IReadOnlyList<AttributeValueDto> Values { get; init; }
}

public sealed class CreateProductAttributeRequest
{
    public required string Name { get; init; }
}

public sealed class UpdateProductAttributeRequest
{
    public required string Name { get; init; }
}

public sealed class AddAttributeValueRequest
{
    public required string Value { get; init; }
}
