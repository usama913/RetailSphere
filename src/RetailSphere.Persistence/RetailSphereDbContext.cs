using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Domain.Auditing;
using RetailSphere.Domain.Catalog;
using RetailSphere.Domain.Customers;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.Domain.Inventory;
using RetailSphere.Domain.Organization;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Persistence;

public sealed class RetailSphereDbContext(DbContextOptions<RetailSphereDbContext> options, IPublisher publisher)
    : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<Branch> Branches => Set<Branch>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Brand> Brands => Set<Brand>();

    public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<RetailSphere.Domain.Catalog.Unit> Units => Set<RetailSphere.Domain.Catalog.Unit>();

    public DbSet<Supplier> Suppliers => Set<Supplier>();

    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();

    public DbSet<StockItem> StockItems => Set<StockItem>();

    public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();

    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Dispatches domain events raised by aggregates in this SaveChanges batch,
    /// AFTER the transaction commits successfully — handlers can safely assume
    /// the state they're reacting to is durable.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregatesWithEvents = ChangeTracker.Entries()
            .Select(e => e.Entity)
            .OfType<AggregateRoot<long>>()
            .Where(a => a.DomainEvents.Count > 0)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        var domainEvents = aggregatesWithEvents.SelectMany(a => a.DomainEvents).ToList();
        aggregatesWithEvents.ForEach(a => a.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }

        return result;
    }
}
