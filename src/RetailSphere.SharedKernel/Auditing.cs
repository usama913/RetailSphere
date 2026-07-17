namespace RetailSphere.SharedKernel;

/// <summary>
/// Implemented by any entity that needs Created/Modified audit stamping.
/// RetailSphere.Persistence applies these automatically via a SaveChanges
/// interceptor — entities never set these fields themselves.
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAtUtc { get; set; }

    long? CreatedBy { get; set; }

    DateTime? ModifiedAtUtc { get; set; }

    long? ModifiedBy { get; set; }
}

/// <summary>
/// Implemented by any entity that supports soft delete. EF Core applies a global
/// query filter (IsDeleted == false) for every type implementing this interface;
/// use IgnoreQueryFilters() explicitly for audit/admin views that need deleted rows.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }

    DateTime? DeletedAtUtc { get; set; }

    long? DeletedBy { get; set; }
}

/// <summary>
/// Implemented by any entity scoped to a specific branch (multi-branch support,
/// per the architecture's decision to keep RetailSphere single-tenant/multi-branch).
/// </summary>
public interface IBranchScoped
{
    long BranchId { get; set; }
}
