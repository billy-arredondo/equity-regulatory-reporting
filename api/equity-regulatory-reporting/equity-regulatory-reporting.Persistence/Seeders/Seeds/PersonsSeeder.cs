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
        string Address,
        string DocumentTypeAbbreviation,
        string DocumentNumber,
        string? EntityCode,
        string? RepresentativeDocumentNumber,
        bool ReportFlag,
        string CountryAbbreviation,
        string InternalLocation
    );

    private async Task SeedPersonsAsync()
    {
        var rows = LoadPersonSeedRows();
        if (rows.Length == 0)
            return;

        var existingDocNumbers = await context.Persons
            .Select(p => p.DocumentNumber)
            .ToHashSetAsync();

        var pending = rows.Where(r => !existingDocNumbers.Contains(r.DocumentNumber)).ToArray();
        if (pending.Length == 0)
            return;

        var countryMap = await context.Countries
            .ToDictionaryAsync(c => c.Abbreviation, c => c.Id);

        var docTypeMap = await context.DocumentTypes
            .ToDictionaryAsync(d => d.Abbreviation, d => d.Id);

        // First pass: insert all persons without resolving representatives
        var personByDocNumber = new Dictionary<string, Person>();

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

            var person = new Person
            {
                Name = row.Name,
                PersonType = Enum.Parse<PersonType>(row.PersonType),
                Ciiu = row.Ciiu,
                Address = row.Address,
                DocumentTypeId = docTypeId,
                DocumentNumber = row.DocumentNumber,
                EntityCode = row.EntityCode,
                ReportFlag = row.ReportFlag,
                CountryId = countryId,
                InternalLocation = row.InternalLocation
            };

            context.Persons.Add(person);
            personByDocNumber[row.DocumentNumber] = person;
        }

        await context.SaveChangesAsync();

        // Second pass: resolve representatives now that all persons have IDs
        var allPersonIds = await context.Persons
            .Where(p => pending.Select(r => r.RepresentativeDocumentNumber)
                .Where(d => d != null)
                .Contains(p.DocumentNumber))
            .ToDictionaryAsync(p => p.DocumentNumber, p => p.Id);

        bool anyUpdated = false;
        foreach (var row in pending.Where(r => r.RepresentativeDocumentNumber is not null))
        {
            if (!personByDocNumber.TryGetValue(row.DocumentNumber, out var person))
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
