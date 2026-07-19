using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Domain.Auditing;
using RetailSphere.Domain.Catalog;
using RetailSphere.Domain.Customers;
using RetailSphere.Domain.Finance;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.Domain.Inventory;
using RetailSphere.Domain.Notifications;
using RetailSphere.Domain.Organization;
using RetailSphere.Domain.Purchasing;
using RetailSphere.Domain.Sales;
using RetailSphere.Persistence.Interceptors;
using RetailSphere.Persistence.Repositories;

namespace RetailSphere.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MySql")
            ?? throw new InvalidOperationException("Connection string 'MySql' was not found in configuration.");

        // Scoped, not Singleton — it depends on the scoped ICurrentUserService
        // (backed by IHttpContextAccessor.HttpContext.User, which only exists per
        // request). A Singleton can't consume a Scoped service; ASP.NET Core's
        // service-provider validation rejects that combination at startup.
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddDbContext<RetailSphereDbContext>((sp, options) =>
        {
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                mySqlOptions => mySqlOptions.EnableRetryOnFailure(maxRetryCount: 3));

            options.AddInterceptors(sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<RetailSphereDbContext>());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<IPurchaseInvoiceRepository, PurchaseInvoiceRepository>();
        services.AddScoped<ISupplierPaymentRepository, SupplierPaymentRepository>();
        services.AddScoped<IStockItemRepository, StockItemRepository>();
        services.AddScoped<IStockTransferRepository, StockTransferRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomerPaymentRepository, CustomerPaymentRepository>();
        services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
        services.AddScoped<ISalesReturnRepository, SalesReturnRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<ICashRegisterSessionRepository, CashRegisterSessionRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        return services;
    }
}
