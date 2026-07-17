using Microsoft.EntityFrameworkCore;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.Domain.IdentityAccess.ValueObjects;
using RetailSphere.Domain.Organization;
using RetailSphere.Persistence;

namespace RetailSphere.API;

/// <summary>
/// Dev-only convenience: Phase 0 has no user-management UI yet (that's Phase 1 —
/// Admin &amp; Identity), so there's no way to create a login through the API itself.
/// This seeds exactly one branch/role/user, idempotently, so `/api/v1/auth/login`
/// has something to authenticate against on a fresh local database. Never wired
/// up outside `Development` — see Program.cs.
/// </summary>
public static class DevelopmentSeeder
{
    public const string SeedAdminEmail = "admin@retailsphere.pk";
    public const string SeedAdminPassword = "Admin@12345";

    public static async Task SeedDevelopmentDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var dbContext = services.GetRequiredService<RetailSphereDbContext>();
        if (await dbContext.Users.AnyAsync())
            return;

        var passwordHasher = services.GetRequiredService<IPasswordHasher>();

        var branch = Branch.Create("Main Branch", "MAIN", address: null, city: "Karachi", taxJurisdictionId: null).Value;
        dbContext.Branches.Add(branch);

        var role = Role.Create("Super Admin", "Full access (development seed only)", isSystemRole: true).Value;
        dbContext.Roles.Add(role);

        // Save first so Branch/Role get their auto-increment Ids before the User references them.
        await dbContext.SaveChangesAsync();

        var email = Email.Create(SeedAdminEmail).Value;
        var hashedPassword = HashedPassword.FromHash(passwordHasher.Hash(SeedAdminPassword)).Value;
        var user = User.Register(email, hashedPassword, "System", "Administrator", branch.Id).Value;
        user.AssignRole(role.Id);
        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync();

        app.Logger.LogWarning(
            "Seeded a development-only login: {Email} / {Password} — do not use this account or ship this seeder beyond local development.",
            SeedAdminEmail, SeedAdminPassword);
    }
}
