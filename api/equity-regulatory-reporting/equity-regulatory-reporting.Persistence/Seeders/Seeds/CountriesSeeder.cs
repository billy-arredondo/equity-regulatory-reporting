using System.Reflection;
using System.Text.Json;
using equity_regulatory_reporting.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Persistence.Seeders;

public partial class DatabaseSeeder
{
    private sealed record CountrySeedRow(string Name, string Abbreviation);

    private async Task SeedCountriesAsync()
    {
        var resourceName = "equity_regulatory_reporting.Persistence.Seeders.Data.countries.json";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream is null)
            return;

        var rows = JsonSerializer.Deserialize<CountrySeedRow[]>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? [];

        var existing = await context.Countries.Select(c => c.Abbreviation).ToHashSetAsync();
        var toAdd = rows
            .Where(r => !existing.Contains(r.Abbreviation))
            .Select(r => new Country { Name = r.Name, Abbreviation = r.Abbreviation });
        context.Countries.AddRange(toAdd);
        await context.SaveChangesAsync();
    }
}
