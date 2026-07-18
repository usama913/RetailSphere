using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Catalog;

/// <summary>
/// A self-referencing category/subcategory tree node (§3: "Aggregate root:
/// Category — self-referencing tree for category/subcategory"). Deliberately a
/// standalone aggregate, not part of Product — Products reference a category by
/// Id only (a plain column, never an EF navigation), keeping the Catalog
/// aggregates independently loadable per the "one repository per aggregate
/// root" rule.
/// </summary>
public sealed class Category : AggregateRoot<long>, IAuditableEntity, ISoftDeletable
{
    public string Name { get; private set; } = default!;

    public string Slug { get; private set; } = default!;

    public long? ParentCategoryId { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public long? DeletedBy { get; set; }

    private Category()
    {
    }

    public static Result<Category> Create(string name, long? parentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Category>(Error.Validation("Category.NameRequired", "Category name is required."));

        return Result.Success(new Category
        {
            Name = name.Trim(),
            Slug = Slugify(name),
            ParentCategoryId = parentCategoryId,
            IsActive = true,
        });
    }

    public Result UpdateDetails(string name, long? parentCategoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation("Category.NameRequired", "Category name is required."));

        if (parentCategoryId.HasValue && parentCategoryId.Value == Id)
            return Result.Failure(Error.Validation("Category.CannotParentToSelf", "A category cannot be its own parent."));

        Name = name.Trim();
        Slug = Slugify(name);
        ParentCategoryId = parentCategoryId;
        return Result.Success();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    // Deliberately simple for v1 — no diacritics/unicode folding, since category
    // names are entered by the merchandising team, not user-generated content.
    private static string Slugify(string name) =>
        name.Trim().ToLowerInvariant().Replace(" ", "-");
}
