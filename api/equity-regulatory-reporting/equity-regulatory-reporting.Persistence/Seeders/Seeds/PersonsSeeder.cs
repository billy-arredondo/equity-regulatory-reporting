using System.Reflection;
using System.Text.Json;
using equity_regulatory_reporting.Domain.Entities;
using equity_regulatory_reporting.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace equity_regulatory_reporting.Persistence.Seeders;

public partial class DatabaseSeeder
{
    private sealed record PersonSeedRow(
        int Id,
        string Name,
        string PersonType,
        string? Ciiu,
        string? Address,
        string DocumentTypeAbbreviation,
        string? DocumentNumber,
        string? EntityCode,
        string? Representative,
        bool ReportFlag,
        string CountryAbbreviation,
        string LocationCode
    );

    private async Task SeedPersonsAsync()
    {
        var rows = LoadPersonSeedRows();
        if (rows.Length == 0)
            return;

        var existingLegacyIds = await context.Persons
            .Where(p => p.LegacyId != null)
            .Select(p => p.LegacyId!.Value)
            .ToHashSetAsync();

        var pending = rows
            .Where(r => !existingLegacyIds.Contains(r.Id))
            .ToArray();
        if (pending.Length == 0)
            return;

        var countryMap = await context.Countries
            .ToDictionaryAsync(c => c.Abbreviation, c => c.Id);

        var docTypeMap = await context.DocumentTypes
            .ToDictionaryAsync(d => d.Abbreviation, d => d.Id);

        var locationMap = await context.Locations
            .ToDictionaryAsync(l => l.Code, l => l.Id);

        // First pass: insert all persons without resolving representatives
        var personByLegacyId = new Dictionary<int, Person>();
        var personByName = new Dictionary<string, Person>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in pending)
        {
            if (!countryMap.TryGetValue(row.CountryAbbreviation, out var countryId))
            {
                logger.LogWarning("Seed: country '{Abbr}' not found, skipping '{Name}'", row.CountryAbbreviation, row.Name);
                continue;
            }

            if (!docTypeMap.TryGetValue(row.DocumentTypeAbbreviation, out var docTypeId))
            {
                logger.LogWarning("Seed: document type '{Abbr}' not found, skipping '{Name}'", row.DocumentTypeAbbreviation, row.Name);
                continue;
            }

            if (!locationMap.TryGetValue(row.LocationCode, out var locationId))
            {
                logger.LogWarning("Seed: location '{Code}' not found, skipping '{Name}'", row.LocationCode, row.Name);
                continue;
            }

            if (!Enum.TryParse<PersonType>(row.PersonType, out var personType))
            {
                logger.LogWarning("Seed: unknown PersonType '{Type}', skipping '{Name}'", row.PersonType, row.Name);
                continue;
            }

            var person = new Person
            {
                LegacyId = row.Id,
                Name = row.Name,
                PersonType = personType,
                Ciiu = row.Ciiu,
                Address = string.IsNullOrEmpty(row.Address) ? null : row.Address,
                DocumentTypeId = docTypeId,
                DocumentNumber = row.DocumentNumber,
                EntityCode = row.EntityCode,
                ReportFlag = row.ReportFlag,
                CountryId = countryId,
                LocationId = locationId
            };

            context.Persons.Add(person);
            personByLegacyId[row.Id] = person;
            personByName.TryAdd(row.Name.Trim(), person);
        }

        await context.SaveChangesAsync();

        // Second pass: resolve representatives by name (Natural persons never have representatives)
        bool anyUpdated = false;
        foreach (var row in pending)
        {
            if (row.Representative is null)
                continue;

            if (!personByLegacyId.TryGetValue(row.Id, out var person))
                continue;

            if (person.PersonType == PersonType.Natural)
                continue;

            var repName = row.Representative.Trim();
            if (personByName.TryGetValue(repName, out var repPerson))
            {
                person.RepresentativeId = repPerson.Id;
            }
            else
            {
                person.RepresentativeDescription = row.Representative;
                logger.LogWarning("Seed: representative '{RepName}' not found for '{Name}', stored as description", repName, row.Name);
            }

            anyUpdated = true;
        }

        if (anyUpdated)
            await context.SaveChangesAsync();
    }

    private static PersonSeedRow[] LoadPersonSeedRows()
    {
        var resourceName = "equity_regulatory_reporting.Persistence.Seeders.Data.persons.json";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream is null)
            return [];

        return JsonSerializer.Deserialize<PersonSeedRow[]>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? [];
    }
}
