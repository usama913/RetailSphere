using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RetailSphere.Common;
using RetailSphere.SharedKernel;

namespace RetailSphere.Persistence.Interceptors;

/// <summary>
/// Stamps Created/Modified audit columns automatically (entities never set these
/// themselves — see IAuditableEntity), and converts hard deletes of ISoftDeletable
/// entities into an update (IsDeleted = true) so "delete" never actually removes a row.
/// </summary>
public sealed class AuditableEntitySaveChangesInterceptor(
    ICurrentUserService currentUserService,
    IDateTimeProvider dateTimeProvider)
    : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            ApplyAuditingAndSoftDelete(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAuditingAndSoftDelete(DbContext context)
    {
        var now = dateTimeProvider.UtcNow;
        var userId = currentUserService.UserId;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry is { Entity: ISoftDeletable softDeletable, State: EntityState.Deleted })
            {
                entry.State = EntityState.Modified;
                softDeletable.IsDeleted = true;
                softDeletable.DeletedAtUtc = now;
                softDeletable.DeletedBy = userId;
            }

            if (entry.Entity is IAuditableEntity auditable)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditable.CreatedAtUtc = now;
                        auditable.CreatedBy = userId;
                        break;
                    case EntityState.Modified:
                        auditable.ModifiedAtUtc = now;
                        auditable.ModifiedBy = userId;
                        break;
                }
            }
        }
    }
}
