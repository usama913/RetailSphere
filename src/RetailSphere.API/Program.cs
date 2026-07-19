using System.Security.Cryptography;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RetailSphere.API;
using RetailSphere.API.Authorization;
using RetailSphere.API.Middleware;
using RetailSphere.Application;
using RetailSphere.Application.Features.Notifications;
using RetailSphere.Infrastructure;
using RetailSphere.Infrastructure.Logging;
using RetailSphere.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    loggerConfiguration.ConfigureRetailSphereLogging(context.Configuration));

// ---- Layer wiring (Clean Architecture composition root) ----
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);

// ---- API versioning ----
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ---- MVC / validation ----
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ---- Swagger ----
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "RetailSphere API", Version = "v1" });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter a valid JWT access token.",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
    };
    options.AddSecurityDefinition("Bearer", jwtScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtScheme, [] } });
});

// ---- JWT authentication (RS256 — validates against the public key only; only
// Infrastructure's JwtTokenService ever touches the private key) ----
var jwtConfig = builder.Configuration.GetSection("Jwt");
var publicKeyPath = jwtConfig["PublicKeyPath"]!;
var rsa = RSA.Create();
rsa.ImportFromPem(File.ReadAllText(publicKeyPath));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtConfig["Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsa),
            ClockSkew = TimeSpan.FromSeconds(30),
        };
    });

builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddAuthorization();

// ---- Rate limiting (§7 — tighter on auth endpoints to blunt credential stuffing) ----
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("auth", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
        }));

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 300,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
            }));
});

// ---- CORS ----
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("RetailSphereUi", policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

// ---- Health checks ----
var healthChecks = builder.Services.AddHealthChecks();
var mySqlConnection = builder.Configuration.GetConnectionString("MySql");
var redisConnection = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrWhiteSpace(mySqlConnection))
    healthChecks.AddMySql(mySqlConnection, name: "mysql");
if (!string.IsNullOrWhiteSpace(redisConnection))
    healthChecks.AddRedis(redisConnection, name: "redis");

// ---- Global exception handling -> ProblemDetails (§6/§7) ----
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Apply any pending EF Core migrations on startup — replaces the old
// EnsureCreatedAsync-in-tests-only approach now that Migrations/ exists.
// Safe to run every boot: MigrateAsync() is a no-op once the database is
// already up to date.
using (var migrationScope = app.Services.CreateScope())
{
    var dbContext = migrationScope.ServiceProvider.GetRequiredService<RetailSphere.Persistence.RetailSphereDbContext>();
    await dbContext.Database.MigrateAsync();
}

// ---- AP/AR Notifications engine (§ Notifications & Alerts) — one daily sweep for
// overdue/due-soon Purchase Invoices and Sales Orders. Only registered when Hangfire
// itself has real storage configured (see AddInfrastructure), same guard it uses.
//
// Uses the service-based IRecurringJobManager (resolved from DI) rather than the
// static RecurringJob helper. AddHangfire(...) registers JobStorage as a *lazy* DI
// singleton — it isn't actually built (and JobStorage.Current isn't set) until
// something resolves it from the container, which normally only happens once the
// Hangfire server's hosted service starts during app.Run(). Calling the static
// RecurringJob.AddOrUpdate here — before app.Run() — throws "Current JobStorage
// instance has not been initialized yet" because JobStorage.Current is still null
// at this point. Resolving IRecurringJobManager forces that same DI-configured
// JobStorage to be built immediately instead.
if (!string.IsNullOrWhiteSpace(mySqlConnection))
{
    using var jobScope = app.Services.CreateScope();
    var recurringJobManager = jobScope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    recurringJobManager.AddOrUpdate<NotificationSweepService>(
        "ap-ar-notification-sweep",
        service => service.RunAsync(CancellationToken.None),
        Cron.Daily);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard("/hangfire");

    // Phase 0 has no registration/admin UI yet — seed one login so there's
    // something to authenticate against locally. Dev-only; see DevelopmentSeeder.cs.
    await app.SeedDevelopmentDataAsync();
}

app.UseExceptionHandler();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors("RetailSphereUi");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// Exposed for WebApplicationFactory-based integration tests.
public partial class Program;
