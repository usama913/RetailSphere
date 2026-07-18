using Microsoft.EntityFrameworkCore;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.Domain.IdentityAccess.ValueObjects;
using RetailSphere.Domain.Organization;
using RetailSphere.Persistence;

namespace RetailSphere.API;

/// <summary>
/// Dev-only convenience: seeds one branch/role/user plus the full permission
/// catalog, idempotently, so `/api/v1/auth/login` has something to authenticate
/// against and Phase 1's Admin &amp; Identity screens have real RBAC data to show
/// on a fresh local database. Never wired up outside `Development` — see Program.cs.
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

        // Permissions are seeded independently of the Users check below so an
        // existing local database picks up any newly-added catalog entries
        // (e.g. after pulling a change that extends PermissionCatalog) without
        // needing a full database reset.
        var existingPermissionCodes = await dbContext.Permissions.Select(p => p.Code).ToListAsync();
        var missingPermissions = PermissionCatalog.All
            .Where(entry => !existingPermissionCodes.Contains(entry.Code))
            .Select(entry => Permission.Create(entry.Id, entry.Code, entry.DisplayName, entry.Module))
            .ToList();

        if (missingPermissions.Count > 0)
        {
            dbContext.Permissions.AddRange(missingPermissions);
            await dbContext.SaveChangesAsync();
        }

        // Backfill: an already-seeded "Super Admin" role from a previous run only has
        // whatever permissions existed in PermissionCatalog at the time it was created.
        // Re-grant anything the catalog has since gained so an existing local database
        // doesn't need a full reset every time PermissionCatalog grows (this is exactly
        // what was missing before — new permission rows were inserted, but the role
        // that's supposed to hold all of them was never updated to include them).
        var superAdminRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "Super Admin");
        if (superAdminRole is not null)
        {
            var grantedAny = false;
            foreach (var entry in PermissionCatalog.All)
            {
                if (superAdminRole.GrantPermission(entry.Id).IsSuccess)
                    grantedAny = true;
            }

            if (grantedAny)
            {
                dbContext.Roles.Update(superAdminRole);
                await dbContext.SaveChangesAsync();
            }
        }

        if (await dbContext.Users.AnyAsync())
            return;

        var passwordHasher = services.GetRequiredService<IPasswordHasher>();

        var branch = Branch.Create("Main Branch", "MAIN", address: null, city: "Karachi", taxJurisdictionId: null).Value;
        dbContext.Branches.Add(branch);

        var role = Role.Create("Super Admin", "Full access (development seed only)", isSystemRole: true).Value;
        foreach (var entry in PermissionCatalog.All)
        {
            role.GrantPermission(entry.Id);
        }
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
