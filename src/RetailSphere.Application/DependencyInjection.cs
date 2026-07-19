using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RetailSphere.Application.Common.Behaviors;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.CustomerPayments.Common;
using RetailSphere.Application.Features.Finance.CashRegister.Common;
using RetailSphere.Application.Features.Finance.Expenses.Common;
using RetailSphere.Application.Features.Inventory.Common;
using RetailSphere.Application.Features.Inventory.StockTransfers.Common;
using RetailSphere.Application.Features.Notifications;
using RetailSphere.Application.Features.Products.Common;
using RetailSphere.Application.Features.PurchaseInvoices.Common;
using RetailSphere.Application.Features.PurchaseOrders.Common;
using RetailSphere.Application.Features.SalesOrders.Common;
using RetailSphere.Application.Features.SalesReturns.Common;
using RetailSphere.Application.Features.SupplierPayments.Common;
using RetailSphere.Application.Features.Users.Common;

namespace RetailSphere.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);
        services.AddAutoMapper(assembly);

        services.AddScoped<UserClaimsResolver>();
        services.AddScoped<AuditLogService>();
        services.AddScoped<UserDtoAssembler>();
        services.AddScoped<ProductDtoAssembler>();
        services.AddScoped<PurchaseOrderDtoAssembler>();
        services.AddScoped<StockItemDtoAssembler>();
        services.AddScoped<StockTransferDtoAssembler>();
        services.AddScoped<SalesOrderDtoAssembler>();
        services.AddScoped<SalesReturnDtoAssembler>();
        services.AddScoped<ExpenseDtoAssembler>();
        services.AddScoped<CashRegisterSessionDtoAssembler>();
        services.AddScoped<PurchaseInvoiceDtoAssembler>();
        services.AddScoped<SupplierPaymentDtoAssembler>();
        services.AddScoped<CustomerPaymentDtoAssembler>();
        services.AddScoped<NotificationSweepService>();

        return services;
    }
}
