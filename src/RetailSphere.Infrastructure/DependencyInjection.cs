using Hangfire;
using Hangfire.MySql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Options;
using RetailSphere.Common;
using RetailSphere.Infrastructure.Email;
using RetailSphere.Infrastructure.Observability;
using RetailSphere.Infrastructure.Security;

namespace RetailSphere.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.SectionName));
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();

        services.AddScoped<IEmailSender, SmtpEmailSender>();

        // Redis — distributed cache, session state, and the backing store for the
        // rate limiter (§7), so all of these behave correctly across multiple API instances.
        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            services.AddStackExchangeRedisCache(options => options.Configuration = redisConnectionString);
        }

        // Hangfire — background jobs (low-stock reorder alerts, scheduled notifications, etc.).
        // Storage lives in the same MySQL instance to avoid standing up another datastore for v1.
        var mySqlConnectionString = configuration.GetConnectionString("MySql");
        if (!string.IsNullOrWhiteSpace(mySqlConnectionString))
        {
            services.AddHangfire(config => config
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseStorage(new MySqlStorage(mySqlConnectionString, new MySqlStorageOptions
                {
                    TablesPrefix = "Hangfire_",
                })));

            services.AddHangfireServer();
        }

        services.AddRetailSphereObservability(configuration);

        return services;
    }
}
