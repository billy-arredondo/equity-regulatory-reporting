using System.Reflection;
using System.Text.Json;
using equity_regulatory_reporting.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Persistence.Seeders;

public partial class DatabaseSeeder
{
    private sealed record GeocodeSeedRow(string Id, string Departamento, string? Provincia, string? Distrito);

    private async Task SeedLocationsAsync()
    {
        if (await context.Locations.AnyAsync())
            return;

        var rows = LoadGeocodeSeedRows();
        foreach (var row in rows)
        {
            context.Locations.Add(new Location
            {
                Code = row.Id,
                Department = row.Departamento,
                Province = row.Provincia ?? string.Empty,
                District = row.Distrito ?? string.Empty
            });
        }

        await context.SaveChangesAsync();
    }

    private static GeocodeSeedRow[] LoadGeocodeSeedRows()
    {
        var resourceName = "equity_regulatory_reporting.Persistence.Seeders.Data.geocode.json";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream is null)
            return [];

        return JsonSerializer.Deserialize<GeocodeSeedRow[]>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? [];
    }
}
