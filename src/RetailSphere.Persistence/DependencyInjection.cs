using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Domain.Auditing;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.Domain.Organization;
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

        return services;
    }
}
