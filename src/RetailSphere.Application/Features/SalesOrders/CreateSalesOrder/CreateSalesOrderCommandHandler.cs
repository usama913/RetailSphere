using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.SalesOrders.Common;
using RetailSphere.Common;
using RetailSphere.Contracts.Sales;
using RetailSphere.Domain.Catalog;
using RetailSphere.Domain.Customers;
using RetailSphere.Domain.Inventory;
using RetailSphere.Domain.Notifications;
using RetailSphere.Domain.Organization;
using RetailSphere.Domain.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SalesOrders.CreateSalesOrder;

/// <summary>
/// The POS checkout handler — the real Inventory integration point for Sales,
/// mirroring ReceivePurchaseOrderLineCommandHandler but decrementing instead of
/// incrementing. Resolves each line's current Sku/description/price/tax from the
/// catalog server-side (the request only carries Ids/quantities/discounts, never
/// prices — see CreateSalesOrderRequest's remarks), pre-checks every line has
/// enough stock before committing to any of them, then builds the whole SalesOrder
/// (Create -> AddLine per item -> SetAmountPaid) and decrements stock line-by-line,
/// all before the first SaveChangesAsync — so a mid-loop failure just discards the
/// pending changes rather than leaving a half-decremented sale.
///
/// Also enforces the Customer Credit Management requirements: once the order's total
/// is known (after its lines are added), if the customer would end up over their
/// CreditLimit, the sale is rejected with SalesOrder.CreditLimitExceeded unless the
/// caller both set OverrideCreditLimit and holds sales.credit.override_limit — in
/// which case the sale proceeds but a Notification and audit entry are raised so the
/// override is traceable.
/// </summary>
public sealed class CreateSalesOrderCommandHandler(
    ISalesOrderRepository salesOrderRepository,
    IBranchRepository branchRepository,
    ICustomerRepository customerRepository,
    IProductRepository productRepository,
    IProductAttributeRepository productAttributeRepository,
    IStockItemRepository stockItemRepository,
    INotificationRepository notificationRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    SalesOrderDtoAssembler salesOrderDtoAssembler)
    : IRequestHandler<CreateSalesOrderCommand, Result<SalesOrderDto>>
{
    private const int MaxOrderNumberAttempts = 5;

    public async Task<Result<SalesOrderDto>> Handle(CreateSalesOrderCommand request, CancellationToken cancellationToken)
    {
        var branch = await branchRepository.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null)
            return Result.Failure<SalesOrderDto>(Error.NotFound("Branch.NotFound", "Branch not found."));

        Customer? customer = null;
        if (request.CustomerId.HasValue)
        {
            customer = await customerRepository.GetByIdAsync(request.CustomerId.Value, cancellationToken);
            if (customer is null)
                return Result.Failure<SalesOrderDto>(Error.NotFound("Customer.NotFound", "Customer not found."));
        }

        var products = await productRepository.GetByVariantIdsAsync(request.Lines.Select(l => l.ProductVariantId).Distinct(), cancellationToken);

        // Resolved once per checkout (not per line) so a cart with several sizes/colors
        // of the same product only costs one extra query — used to spell out each
        // sold variant's attribute values (e.g. "Size: 42, Color: Red") on the
        // DescriptionSnapshot below, which is what shows on the printed receipt, the
        // Sales Order history, and — most importantly — the Returns screen's line
        // picker, so staff can tell exactly which variant a customer is returning
        // instead of just an opaque SKU code.
        var attributes = await productAttributeRepository.GetAllAsync(cancellationToken);

        // Resolve every line's variant up front (and pre-check stock) before touching
        // anything, so a bad line fails the whole checkout instead of a partial sale.
        var resolvedLines = new List<(Product Product, ProductVariant Variant, decimal Quantity, decimal DiscountAmount)>();
        foreach (var line in request.Lines)
        {
            var product = products.FirstOrDefault(p => p.Variants.Any(v => v.Id == line.ProductVariantId));
            var variant = product?.Variants.FirstOrDefault(v => v.Id == line.ProductVariantId);
            if (product is null || variant is null)
                return Result.Failure<SalesOrderDto>(Error.NotFound("Product.VariantNotFound", $"Product variant {line.ProductVariantId} not found."));

            var stockItem = await stockItemRepository.GetByVariantAndBranchAsync(variant.Id, request.BranchId, cancellationToken);
            var available = stockItem?.QuantityOnHand ?? 0;
            if (available < line.Quantity)
                return Result.Failure<SalesOrderDto>(Error.Validation(
                    "SalesOrder.InsufficientStock",
                    $"Not enough stock for '{variant.Sku}' at this branch (available: {available}, requested: {line.Quantity})."));

            resolvedLines.Add((product, variant, line.Quantity, line.DiscountAmount));
        }

        SalesOrder salesOrder;
        var creditLimitBreached = false;
        var attempt = 1;
        while (true)
        {
            var orderNumber = await GenerateOrderNumberAsync(cancellationToken);

            var createResult = SalesOrder.Create(
                orderNumber, request.BranchId, request.CustomerId, currentUserService.UserId,
                DateTime.UtcNow, request.PaymentMethod, request.OrderDiscountAmount, request.Notes,
                request.PaymentTerms, request.DueDate);
            if (createResult.IsFailure)
                return Result.Failure<SalesOrderDto>(createResult.Error);

            salesOrder = createResult.Value;

            foreach (var (product, variant, quantity, discountAmount) in resolvedLines)
            {
                var descriptionSnapshot = BuildDescriptionSnapshot(product, variant, attributes);
                var addResult = salesOrder.AddLine(
                    product.Id, variant.Id, variant.Sku, descriptionSnapshot,
                    quantity, variant.Price, variant.TaxRate, variant.TaxType, discountAmount, variant.CostPrice ?? 0);
                if (addResult.IsFailure)
                    return Result.Failure<SalesOrderDto>(addResult.Error);
            }

            var paymentResult = salesOrder.SetAmountPaid(request.AmountPaid);
            if (paymentResult.IsFailure)
                return Result.Failure<SalesOrderDto>(paymentResult.Error);

            if (customer is not null && customer.CreditLimit.HasValue && salesOrder.OutstandingBalance > 0)
            {
                var existingOutstanding = (await salesOrderRepository.GetOutstandingByCustomerAsync(customer.Id, cancellationToken))
                    .Sum(o => o.OutstandingBalance);
                var prospectiveTotal = existingOutstanding + salesOrder.OutstandingBalance;

                if (prospectiveTotal > customer.CreditLimit.Value)
                {
                    if (!request.OverrideCreditLimit)
                        return Result.Failure<SalesOrderDto>(Error.Conflict(
                            "SalesOrder.CreditLimitExceeded",
                            $"This sale would bring {customer.Name}'s outstanding balance to {prospectiveTotal:0.00}, over their {customer.CreditLimit.Value:0.00} credit limit."));

                    if (!currentUserService.HasPermission("sales.credit.override_limit"))
                        return Result.Failure<SalesOrderDto>(Error.Unauthorized(
                            "SalesOrder.CreditLimitOverrideDenied", "You don't have permission to override a customer's credit limit."));

                    creditLimitBreached = true;
                }
            }

            salesOrderRepository.Add(salesOrder);

            try
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                break;
            }
            catch (Exception ex) when (attempt < MaxOrderNumberAttempts && IsDuplicateOrderNumberViolation(ex))
            {
                salesOrderRepository.Remove(salesOrder);
                attempt++;
            }
        }

        // Stock decrement — after the order itself is safely persisted (so it has a
        // real Id for the adjustment reason text), one AdjustQuantity per line.
        foreach (var line in salesOrder.Lines)
        {
            var stockItem = await stockItemRepository.GetByVariantAndBranchAsync(line.ProductVariantId, request.BranchId, cancellationToken);
            if (stockItem is null)
                return Result.Failure<SalesOrderDto>(Error.NotFound("StockItem.NotFound", $"Stock record for '{line.SkuSnapshot}' not found."));

            var adjustResult = stockItem.AdjustQuantity(-line.Quantity, $"Sold on order '{salesOrder.OrderNumber}'.", "Sale");
            if (adjustResult.IsFailure)
                return Result.Failure<SalesOrderDto>(adjustResult.Error);

            stockItemRepository.Update(stockItem);
        }

        auditLogService.Log("SalesOrder", salesOrder.Id.ToString(), "Created", $"Completed sale '{salesOrder.OrderNumber}' for {salesOrder.TotalAmount:0.00}.");

        if (creditLimitBreached && customer is not null)
        {
            auditLogService.Log(
                "SalesOrder", salesOrder.Id.ToString(), "CreditLimitOverridden",
                $"Sale '{salesOrder.OrderNumber}' exceeded customer '{customer.Name}''s credit limit of {customer.CreditLimit:0.00} and was overridden by {currentUserService.Email}.");

            notificationRepository.Add(Notification.Create(
                "CustomerCreditLimitExceeded",
                "Critical",
                $"Sale '{salesOrder.OrderNumber}' pushed customer '{customer.Name}' over their credit limit of {customer.CreditLimit:0.00} (override approved by {currentUserService.Email}).",
                "Customer",
                customer.Id,
                userId: null));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await salesOrderDtoAssembler.ToDtoAsync(salesOrder, cancellationToken);
        return Result.Success(dto);
    }

    /// <summary>
    /// "{Product name} ({Sku}) — {Attribute: Value, ...}" — e.g. "Rolex Green Clean
    /// Dial (P6-V04) — Size: 42". Only appended when the variant actually has
    /// attribute values selected; a plain variant keeps the original "Name (Sku)" form.
    /// </summary>
    private static string BuildDescriptionSnapshot(Product product, ProductVariant variant, IReadOnlyList<ProductAttribute> attributes)
    {
        var baseDescription = $"{product.Name} ({variant.Sku})";

        if (variant.AttributeValueIds.Count == 0)
            return baseDescription;

        var parts = new List<string>();
        foreach (var attribute in attributes)
        {
            var matchedValues = attribute.Values
                .Where(v => variant.AttributeValueIds.Contains(v.Id))
                .Select(v => v.Value)
                .ToList();

            if (matchedValues.Count > 0)
                parts.Add($"{attribute.Name}: {string.Join(", ", matchedValues)}");
        }

        return parts.Count > 0 ? $"{baseDescription} — {string.Join(", ", parts)}" : baseDescription;
    }

    /// <summary>Same duplicate-detection approach as CreatePurchaseOrderCommandHandler.IsDuplicatePoNumberViolation — see its remarks.</summary>
    private static bool IsDuplicateOrderNumberViolation(Exception ex) =>
        ex.GetType().Name == "DbUpdateException"
        && (ex.InnerException?.Message.Contains("IX_SalesOrders_OrderNumber", StringComparison.OrdinalIgnoreCase) == true
            || ex.InnerException?.Message.Contains("Duplicate entry", StringComparison.OrdinalIgnoreCase) == true);

    /// <summary>Month-scoped order number (e.g. "SO-202607-0001") — same pattern as CreatePurchaseOrderCommandHandler.GeneratePoNumberAsync.</summary>
    private async Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken)
    {
        var prefix = $"SO-{DateTime.UtcNow:yyyyMM}-";
        var sequence = 1;
        while (true)
        {
            var candidate = $"{prefix}{sequence:0000}";
            if (!await salesOrderRepository.OrderNumberExistsAsync(candidate, cancellationToken))
                return candidate;

            sequence++;
        }
    }
}
