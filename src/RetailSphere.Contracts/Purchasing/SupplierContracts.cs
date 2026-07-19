namespace RetailSphere.Contracts.Purchasing;

public sealed class SupplierDto
{
    public required long Id { get; init; }

    public required string Name { get; init; }

    public string? ContactPerson { get; init; }

    public string? Email { get; init; }

    public string? Phone { get; init; }

    public string? Address { get; init; }

    public string? TaxNumber { get; init; }

    public decimal? CreditLimit { get; init; }

    public required string PaymentTerms { get; init; }

    public required bool IsActive { get; init; }
}

public sealed class CreateSupplierRequest
{
    public required string Name { get; init; }

    public string? ContactPerson { get; init; }

    public string? Email { get; init; }

    public string? Phone { get; init; }

    public string? Address { get; init; }

    public string? TaxNumber { get; init; }

    public decimal? CreditLimit { get; init; }

    public string? PaymentTerms { get; init; }
}

public sealed class UpdateSupplierRequest
{
    public required string Name { get; init; }

    public string? ContactPerson { get; init; }

    public string? Email { get; init; }

    public string? Phone { get; init; }

    public string? Address { get; init; }

    public string? TaxNumber { get; init; }
}

public sealed class UpdateSupplierCreditTermsRequest
{
    public decimal? CreditLimit { get; init; }

    public string? PaymentTerms { get; init; }
}
