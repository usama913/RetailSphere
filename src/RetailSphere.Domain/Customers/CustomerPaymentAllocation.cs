using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Customers;

/// <summary>
/// Child entity of CustomerPayment — records that some portion of a single payment
/// was applied to one specific Sales Order. Lets a customer's one cheque/transfer
/// be split across several outstanding invoices in one go (per the Customer Payment
/// Module spec: "apply a payment to one or multiple invoices").
/// </summary>
public sealed class CustomerPaymentAllocation : Entity<long>
{
    public long CustomerPaymentId { get; private set; }

    public long SalesOrderId { get; private set; }

    public decimal Amount { get; private set; }

    private CustomerPaymentAllocation()
    {
    }

    internal static CustomerPaymentAllocation Create(long customerPaymentId, long salesOrderId, decimal amount) => new()
    {
        CustomerPaymentId = customerPaymentId,
        SalesOrderId = salesOrderId,
        Amount = amount,
    };
}
