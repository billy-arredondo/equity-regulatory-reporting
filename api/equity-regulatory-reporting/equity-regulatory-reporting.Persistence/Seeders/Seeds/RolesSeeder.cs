using equity_regulatory_reporting.Domain.Enums;
using equity_regulatory_reporting.Persistence.Identity;
using Microsoft.Extensions.Logging;

namespace equity_regulatory_reporting.Persistence.Seeders;

public partial class DatabaseSeeder
{
    private async Task SeedRolesAsync()
    {
        ApplicationRole[] roles =
        [
            new()
            {
                Name = "Admin",
                NormalizedName = "ADMIN",
                Permissions = Permission.Admin
            },
            new()
            {
                Name = "Guest",
                NormalizedName = "GUEST",
                Permissions =
                    Permission.PersonRead |
                    Permission.ParticipationRead |
                    Permission.BoardRead |
                    Permission.ReportRead |
                    Permission.CountryRead |
                    Permission.DocumentTypeRead |
                    Permission.PositionRead
            }
        ];

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
}
