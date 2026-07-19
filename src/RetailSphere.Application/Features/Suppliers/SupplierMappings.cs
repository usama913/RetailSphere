using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;

namespace RetailSphere.Application.Features.Suppliers;

internal static class SupplierMappings
{
    public static SupplierDto ToDto(Supplier supplier) => new()
    {
        Id = supplier.Id,
        Name = supplier.Name,
        ContactPerson = supplier.ContactPerson,
        Email = supplier.Email,
        Phone = supplier.Phone,
        Address = supplier.Address,
        TaxNumber = supplier.TaxNumber,
        CreditLimit = supplier.CreditLimit,
        PaymentTerms = supplier.PaymentTerms,
        IsActive = supplier.IsActive,
    };
}
