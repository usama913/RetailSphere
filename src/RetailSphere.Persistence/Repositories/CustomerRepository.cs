using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Customers;

namespace RetailSphere.Persistence.Repositories;

public sealed class CustomerRepository(RetailSphereDbContext dbContext) : ICustomerRepository
{
    public Task<Customer?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Customer>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Customers.AsQueryable();
        if (!includeInactive) query = query.Where(c => c.IsActive);
        return await query.OrderBy(c => c.Name).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Ignores the soft-delete query filter — same reasoning as BrandRepository.NameExistsAsync:
    /// Customers.HasQueryFilter(c => !c.IsDeleted) would otherwise let a soft-deleted
    /// customer's phone number look free forever. Only called when a phone is supplied.
    /// </summary>
    public Task<bool> PhoneExistsAsync(string phone, long? excludeId = null, CancellationToken cancellationToken = default) =>
        dbContext.Customers.IgnoreQueryFilters().AnyAsync(c => c.Phone == phone && (excludeId == null || c.Id != excludeId.Value), cancellationToken);

    public void Add(Customer customer) => dbContext.Customers.Add(customer);

    public void Update(Customer customer) => dbContext.Customers.Update(customer);

    public void Remove(Customer customer) => dbContext.Customers.Remove(customer);
}
