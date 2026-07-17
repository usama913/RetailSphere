# RetailSphere — Enterprise Retail Management System

A multi-branch retail management platform (shoes, watches, clothing, accessories) built on
ASP.NET Core 9, Clean Architecture, DDD, CQRS/MediatR, EF Core + MySQL 8.0, Redis, Hangfire,
and a MudBlazor frontend. See `RetailSphere-Architecture.md` (delivered separately during the
Phase 1 design discussion) for the full solution architecture, business domain analysis, DB
design, and roadmap this codebase implements.

## Status: Phase 0 — Foundations

This is the initial scaffold: solution structure, SharedKernel, an Identity & Access auth
skeleton (login/refresh/logout with JWT + rotating refresh tokens), EF Core + MySQL wiring,
Redis/Hangfire/Serilog/OpenTelemetry, the API composition root, and a MudBlazor UI shell
(theme, drawer/sidebar, JWT auth flow). Business modules (Catalog, Inventory, Purchasing,
Sales/POS, Customers, Finance, Reporting) are built out phase-by-phase from here per the
roadmap in the architecture doc.

## Solution layout

```
src/
  RetailSphere.SharedKernel   Entity/AggregateRoot/ValueObject/Result base types
  RetailSphere.Domain         Aggregates, domain events, repository interfaces
  RetailSphere.Application    CQRS commands/queries (MediatR), validation, DI wiring
  RetailSphere.Contracts      Wire DTOs shared by API and UI
  RetailSphere.Common         Cross-cutting abstractions (ICurrentUserService, clock)
  RetailSphere.Infrastructure JWT/password hashing, Redis, Hangfire, Serilog, OpenTelemetry
  RetailSphere.Persistence    EF Core DbContext, configurations, repositories, MySQL provider
  RetailSphere.API            ASP.NET Core Web API — composition root
ui/
  RetailSphere.UI             Blazor WebAssembly + MudBlazor frontend
tests/
  RetailSphere.UnitTests          Domain/Application logic — no I/O
  RetailSphere.IntegrationTests   API + Persistence against real MySQL/Redis (Testcontainers)
```

## Running locally

### Prerequisites
- .NET 9 SDK
- Docker (for MySQL/Redis, and for the integration test suite's Testcontainers)

### Option A — Docker Compose (API + UI + MySQL + Redis)

```
docker compose up --build
```

- API: http://localhost:8080/swagger
- UI: http://localhost:8081

### Option B — Run locally against Docker-hosted MySQL/Redis only

```
docker compose up -d mysql redis
dotnet run --project src/RetailSphere.API
dotnet run --project ui/RetailSphere.UI
```

The API's `appsettings.json` points at `localhost` MySQL/Redis by default for this mode.

### Dev JWT keys

`src/RetailSphere.API/keys/` holds a throwaway RSA-2048 dev keypair (gitignored) used to sign
JWTs locally. See `keys/README.md` — regenerate anytime with the two `openssl` commands there.
Never reuse this keypair anywhere beyond a developer's own machine.

### First migration

No EF Core migration exists yet (this scaffold was built without the .NET SDK available in the
authoring environment). Before the first real run against a persistent database, generate one:

```
dotnet ef migrations add InitialCreate --project src/RetailSphere.Persistence --startup-project src/RetailSphere.API
dotnet ef database update --project src/RetailSphere.Persistence --startup-project src/RetailSphere.API
```

The integration test suite doesn't need this — it calls `EnsureCreatedAsync()` against a
throwaway Testcontainers MySQL instance instead (see `RetailSphereApiFactory`).

## Tests

```
dotnet test
```

Integration tests spin up real MySQL and Redis containers via Testcontainers — Docker must be
running.
