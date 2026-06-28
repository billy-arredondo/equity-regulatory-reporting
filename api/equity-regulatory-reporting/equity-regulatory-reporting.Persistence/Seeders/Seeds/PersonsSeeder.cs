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
        string Name,
        string PersonType,
        string? Ciiu,
        string? Address,
        string DocumentTypeAbbreviation,
        string? DocumentNumber,
        string? EntityCode,
        string? RepresentativeDocumentNumber,
        bool ReportFlag,
        string CountryAbbreviation,
        string LocationCode
    );

    private async Task SeedPersonsAsync()
    {
        var rows = LoadPersonSeedRows();
        if (rows.Length == 0)
            return;

        var existingDocNumbers = await context.Persons
            .Where(p => p.DocumentNumber != null)
            .Select(p => p.DocumentNumber!)
            .ToHashSetAsync();

        var pending = rows
            .Where(r => r.DocumentNumber == null || !existingDocNumbers.Contains(r.DocumentNumber))
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
        var personByDocNumber = new Dictionary<string, Person>(StringComparer.OrdinalIgnoreCase);

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
                Name = row.Name,
                PersonType = personType,
                Ciiu = row.Ciiu,
                Address = row.Address,
                DocumentTypeId = docTypeId,
                DocumentNumber = row.DocumentNumber,
                EntityCode = row.EntityCode,
                ReportFlag = row.ReportFlag,
                CountryId = countryId,
                LocationId = locationId
            };

            context.Persons.Add(person);
            if (row.DocumentNumber is not null)
                personByDocNumber[row.DocumentNumber] = person;
        }

        await context.SaveChangesAsync();

        // Second pass: resolve representatives now that all persons have IDs
        var repDocNumbers = pending
            .Select(r => r.RepresentativeDocumentNumber)
            .Where(d => d != null)
            .ToHashSet()!;

        var allPersonIds = await context.Persons
            .Where(p => p.DocumentNumber != null && repDocNumbers.Contains(p.DocumentNumber))
            .ToDictionaryAsync(p => p.DocumentNumber!, p => p.Id);

        bool anyUpdated = false;
        foreach (var row in pending.Where(r => r.RepresentativeDocumentNumber is not null))
        {
            if (row.DocumentNumber is null || !personByDocNumber.TryGetValue(row.DocumentNumber, out var person))
                continue;

            if (!allPersonIds.TryGetValue(row.RepresentativeDocumentNumber!, out var repId))
            {
                logger.LogWarning("Seed: representative '{RepDoc}' not found for '{Name}'", row.RepresentativeDocumentNumber, row.Name);
                continue;
            }

            person.RepresentativeId = repId;
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
