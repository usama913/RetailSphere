using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace RetailSphere.Infrastructure.Logging;

public static class SerilogExtensions
{
    /// <summary>
    /// Structured Serilog configuration (§10 of the architecture doc): console sink
    /// for local dev, Seq (or swap for your centralized sink of choice — Elasticsearch/
    /// Grafana Loki) in every other environment. Enriched with correlation-friendly
    /// context so BranchId/UserId/CorrelationId show up on every log line once the
    /// corresponding enrichers are wired in the API's request pipeline.
    /// </summary>
    public static LoggerConfiguration ConfigureRetailSphereLogging(this LoggerConfiguration loggerConfiguration, IConfiguration configuration)
    {
        var seqUrl = configuration["Observability:SeqUrl"];

        loggerConfiguration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "RetailSphere.API")
            .WriteTo.Console();

        if (!string.IsNullOrWhiteSpace(seqUrl))
            loggerConfiguration.WriteTo.Seq(seqUrl);

        return loggerConfiguration;
    }
}
