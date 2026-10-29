using Byway.Infrastructure.Identity;
using Byway.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Byway.Infrastructure.Seed;

/// <summary>
/// Seeds Identity roles, a default Admin account, and sample catalog data.
/// Call SeedAsync from the one-shot database initialization mode after migrations succeed.
///
/// Admin credentials come from configuration (appsettings.Development.json
/// or environment variables), never hardcoded here — see README for the
/// exact settings expected. This file intentionally does not commit a real
/// password; SeedAdminOptions falls back to a placeholder that must be
/// overridden per-environment.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();

        await SeedRolesAsync(services);
        await SeedAdminAsync(services);
        await SampleCatalogSeeder.SeedAsync(db);
    }

    private static async Task SeedRolesAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in new[] { IdentityRoles.Admin, IdentityRoles.Student })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task SeedAdminAsync(IServiceProvider services)
    {
        var config = services.GetRequiredService<IConfiguration>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        var adminEmail = config["SeedAdmin:Email"] ?? "admin@byway.local";
        var adminPassword = config["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(adminPassword))
        {
            // Deliberately does not fall back to a hardcoded password —
            // set SeedAdmin:Password in appsettings.Development.json (which
            // should stay out of source control) or an environment variable.
            return;
        }

        if (await userManager.FindByEmailAsync(adminEmail) is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Byway",
            LastName = "Admin",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, IdentityRoles.Admin);
    }

}
