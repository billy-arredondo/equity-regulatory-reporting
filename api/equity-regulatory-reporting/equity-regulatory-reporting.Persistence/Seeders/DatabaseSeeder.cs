using equity_regulatory_reporting.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace equity_regulatory_reporting.Persistence.Seeders;

public partial class DatabaseSeeder(
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
        await SeedCountriesAsync();
        await SeedDocumentTypesAsync();
        await SeedLocationsAsync();
        await SeedPersonsAsync();
        await SeedDefaultAdminAsync();
    }
}
