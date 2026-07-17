namespace RetailSphere.Application.Common.Interfaces;

/// <summary>
/// Implemented by RetailSphere.Persistence's DbContext. Command handlers call
/// SaveChangesAsync exactly once, at the end of the use case, so an entire
/// command is one atomic unit — repositories only stage changes in memory.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
