using equity_regulatory_reporting.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Persistence.Seeders;

public partial class DatabaseSeeder
{
    private async Task SeedLocationsAsync()
    {
        if (await context.Locations.AnyAsync())
            return;

        context.Locations.Add(new Location
        {
            Code = "999999",
            Department = "Extranjero",
            Province = "Extranjero",
            District = "Extranjero"
        });

        await context.SaveChangesAsync();
    }
}
