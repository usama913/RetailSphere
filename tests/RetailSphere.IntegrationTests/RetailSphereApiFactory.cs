using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RetailSphere.Persistence;
using Testcontainers.MySql;
using Testcontainers.Redis;
using Xunit;

namespace RetailSphere.IntegrationTests;

/// <summary>
/// Spins up real MySQL and Redis containers (§10 of the architecture doc: integration
/// tests run against real dependencies via Testcontainers, not mocks/in-memory fakes)
/// and points the API at them, plus a throwaway RSA keypair generated fresh per test
/// run so tests never depend on the dev keys checked out on disk.
/// </summary>
public sealed class RetailSphereApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _mySqlContainer = new MySqlBuilder()
        .WithImage("mysql:8.0")
        .WithDatabase("retailsphere_test")
        .WithUsername("retailsphere")
        .WithPassword("changeme")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:7-alpine")
        .Build();

    private string _tempDirectory = default!;

    public async Task InitializeAsync()
    {
        await Task.WhenAll(_mySqlContainer.StartAsync(), _redisContainer.StartAsync());

        _tempDirectory = Directory.CreateTempSubdirectory("retailsphere-tests-").FullName;
        GenerateTestRsaKeyPair(_tempDirectory);

        // Phase 0 has no EF Core migrations yet (no SDK was available in the scaffolding
        // environment to generate the initial one) — EnsureCreatedAsync builds the schema
        // straight from the current model so tests aren't blocked on that. Switch this to
        // `dbContext.Database.MigrateAsync()` once the first migration exists.
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RetailSphereDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        await _mySqlContainer.DisposeAsync();
        await _redisContainer.DisposeAsync();

        if (Directory.Exists(_tempDirectory))
            Directory.Delete(_tempDirectory, recursive: true);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:MySql"] = _mySqlContainer.GetConnectionString(),
                ["ConnectionStrings:Redis"] = _redisContainer.GetConnectionString(),
                ["Jwt:PrivateKeyPath"] = Path.Combine(_tempDirectory, "jwt-private.pem"),
                ["Jwt:PublicKeyPath"] = Path.Combine(_tempDirectory, "jwt-public.pem"),
            });
        });
    }

    private static void GenerateTestRsaKeyPair(string directory)
    {
        using var rsa = RSA.Create(2048);
        File.WriteAllText(Path.Combine(directory, "jwt-private.pem"), rsa.ExportRSAPrivateKeyPem());
        File.WriteAllText(Path.Combine(directory, "jwt-public.pem"), rsa.ExportRSAPublicKeyPem());
    }
}
