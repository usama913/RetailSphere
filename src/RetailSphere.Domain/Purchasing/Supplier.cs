using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Purchasing;

/// <summary>Aggregate root: Supplier — Purchase Orders reference a supplier by Id only.</summary>
public sealed class Supplier : AggregateRoot<long>, IAuditableEntity, ISoftDeletable
{
    public string Name { get; private set; } = default!;

    public string? ContactPerson { get; private set; }

    public string? Email { get; private set; }

    public string? Phone { get; private set; }

    public string? Address { get; private set; }

    /// <summary>Tax registration number (e.g. NTN) — optional, used on purchase documents.</summary>
    public string? TaxNumber { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAtUtc { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public long? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public long? DeletedBy { get; set; }

    private Supplier()
    {
    }

    public static Result<Supplier> Create(
        string name,
        string? contactPerson,
        string? email,
        string? phone,
        string? address,
        string? taxNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Supplier>(Error.Validation("Supplier.NameRequired", "Supplier name is required."));

        return Result.Success(new Supplier
        {
            Name = name.Trim(),
            ContactPerson = contactPerson,
            Email = email,
            Phone = phone,
            Address = address,
            TaxNumber = taxNumber,
            IsActive = true,
        });
    }

    public Result UpdateDetails(
        string name,
        string? contactPerson,
        string? email,
        string? phone,
        string? address,
        string? taxNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation("Supplier.NameRequired", "Supplier name is required."));

        Name = name.Trim();
        ContactPerson = contactPerson;
        Email = email;
        Phone = phone;
        Address = address;
        TaxNumber = taxNumber;
        return Result.Success();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
