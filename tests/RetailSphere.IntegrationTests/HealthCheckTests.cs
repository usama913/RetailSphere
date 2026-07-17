using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace RetailSphere.IntegrationTests;

public sealed class HealthCheckTests(RetailSphereApiFactory factory) : IClassFixture<RetailSphereApiFactory>
{
    [Fact]
    public async Task Health_endpoint_returns_ok_when_mysql_and_redis_are_reachable()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_with_unknown_email_returns_unauthorized_not_a_500()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            Email = "nobody@retailsphere.test",
            Password = "wrong-password",
        });

        // Proves the global exception handler / Result-mapping pipeline (§6, §7) is wired:
        // an expected auth failure must come back as 401, never as an unhandled 500.
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
