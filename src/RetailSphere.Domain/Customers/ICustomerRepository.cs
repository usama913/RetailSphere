namespace RetailSphere.Domain.Customers;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Customer>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    /// <summary>Ignores the soft-delete filter — see CustomerRepository.PhoneExistsAsync. Only enforced when a phone is supplied.</summary>
    Task<bool> PhoneExistsAsync(string phone, long? excludeId = null, CancellationToken cancellationToken = default);

    void Add(Customer customer);

    void Update(Customer customer);

    void Remove(Customer customer);
}
