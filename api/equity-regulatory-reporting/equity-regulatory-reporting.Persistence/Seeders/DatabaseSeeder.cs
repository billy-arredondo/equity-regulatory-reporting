using equity_regulatory_reporting.Domain.Entities;
using equity_regulatory_reporting.Domain.Enums;
using equity_regulatory_reporting.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace equity_regulatory_reporting.Persistence.Seeders;

public class DatabaseSeeder(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IConfiguration configuration,
    ILogger<DatabaseSeeder> logger)
{
    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedPositionsAsync();
        await SeedDefaultAdminAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[]
        {
            new ApplicationRole { Name = "Admin", NormalizedName = "ADMIN", Permissions = Permission.Admin },
            new ApplicationRole
            {
                Name = "Guest",
                NormalizedName = "GUEST",
                Permissions = Permission.PersonRead | Permission.ParticipationRead | Permission.BoardRead | Permission.ReportRead
            }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                var result = await roleManager.CreateAsync(role);
                if (!result.Succeeded)
                    logger.LogWarning("Failed to create role {Role}: {Errors}", role.Name, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    private async Task SeedPositionsAsync()
    {
        if (!await context.Positions.AnyAsync(p => p.Name == "No position"))
        {
            context.Positions.Add(new Position { Name = "No position" });
            await context.SaveChangesAsync();
        }
    }

    private async Task SeedDefaultAdminAsync()
    {
        var adminEmail = configuration["Seed:AdminEmail"];
        var adminPassword = configuration["Seed:AdminPassword"];

        if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            return;

        if (await userManager.FindByEmailAsync(adminEmail) is not null)
            return;

        var admin = new ApplicationUser
        {
            Id = Guid.CreateVersion7(),
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "System"
        };

        var result = await userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, "Admin");
        else
            logger.LogWarning("Failed to create default admin: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}
