using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Customers;

/// <summary>
/// Aggregate root: Customer — a standalone master-data entity, same shape as
/// Supplier (see RetailSphere.Domain.Purchasing.Supplier). Sales Orders/POS will
/// reference a customer by Id only, never by navigation, matching how
/// PurchaseOrder references Supplier.
/// </summary>
public sealed class Customer : AggregateRoot<long>, IAuditableEntity, ISoftDeletable
{
    public string Name { get; private set; } = default!;

    public string? Phone { get; private set; }

    public string? Email { get; private set; }

    public string? Address { get; private set; }

    /// <summary>
    /// Maximum total outstanding balance (sum of unpaid/underpaid Sales Order
    /// balances) this customer is allowed to carry before checkout should warn/block
    /// — see CreateSalesOrderCommandHandler's credit-limit check. Null means no
    /// limit is enforced (unlimited credit).
    /// </summary>
    public decimal? CreditLimit { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public long? DeletedBy { get; set; }

    private Customer()
    {
    }

    public static Result<Customer> Create(string name, string? phone, string? email, string? address, decimal? creditLimit = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Customer>(Error.Validation("Customer.NameRequired", "Customer name is required."));

        if (creditLimit.HasValue && creditLimit.Value < 0)
            return Result.Failure<Customer>(Error.Validation("Customer.InvalidCreditLimit", "Credit limit cannot be negative."));

        return Result.Success(new Customer
        {
            Name = name.Trim(),
            Phone = phone,
            Email = email,
            Address = address,
            CreditLimit = creditLimit,
            IsActive = true,
        });
    }

    public Result UpdateDetails(string name, string? phone, string? email, string? address)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation("Customer.NameRequired", "Customer name is required."));

        Name = name.Trim();
        Phone = phone;
        Email = email;
        Address = address;
        return Result.Success();
    }

    /// <summary>Separated from UpdateDetails so a credit-limit change is its own auditable action.</summary>
    public Result UpdateCreditLimit(decimal? creditLimit)
    {
        if (creditLimit.HasValue && creditLimit.Value < 0)
            return Result.Failure(Error.Validation("Customer.InvalidCreditLimit", "Credit limit cannot be negative."));

        CreditLimit = creditLimit;
        return Result.Success();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
