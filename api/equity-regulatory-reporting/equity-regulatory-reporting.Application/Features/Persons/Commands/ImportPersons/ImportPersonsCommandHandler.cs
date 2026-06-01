using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Persons.Import;
using equity_regulatory_reporting.Domain.Entities;
using equity_regulatory_reporting.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.Persons.Commands.ImportPersons;

public class ImportPersonsCommandHandler(
    IRepository<Person> personRepository,
    IRepository<DocumentType> documentTypeRepository,
    IRepository<Country> countryRepository)
    : IRequestHandler<ImportPersonsCommand, ImportResult>
{
    public async Task<ImportResult> Handle(ImportPersonsCommand request, CancellationToken cancellationToken)
    {
        // Preload lookups
        var documentTypes = await documentTypeRepository.Query()
            .Include(d => d.AllowedPersonTypes)
            .ToListAsync(cancellationToken);
        var docTypeByAbbreviation = documentTypes.ToDictionary(
            d => d.Abbreviation, d => d, StringComparer.OrdinalIgnoreCase);

        var countryIdByAbbreviation = await countryRepository.Query()
            .ToDictionaryAsync(
                c => c.Abbreviation, c => c.Id,
                StringComparer.OrdinalIgnoreCase, cancellationToken);

        // In-batch resolution map: DocumentNumber → (Id, PersonType)
        // Includes existing DB persons; updated as valid rows are staged
        var personMap = await personRepository.Query()
            .ToDictionaryAsync(
                p => p.DocumentNumber,
                p => new PersonLookup(p.Id, p.PersonType),
                StringComparer.OrdinalIgnoreCase,
                cancellationToken);

        var errors = new List<ImportError>();
        var staged = new List<Person>();

        // Seed seen set from DB so duplicates against existing records are caught
        var seenDocumentNumbers = new HashSet<string>(personMap.Keys, StringComparer.OrdinalIgnoreCase);

        foreach (var row in request.Rows)
        {
            var rowErrors = new List<ImportError>();
            DocumentType? documentType = null;
            Guid? countryId = null;
            Guid? representativeId = null;
            PersonType personType = default;
            bool reportFlag = false;

            // PersonType
            if (string.IsNullOrWhiteSpace(row.PersonType))
                rowErrors.Add(new ImportError(row.LineNumber, "PersonType", "PersonType is required."));
            else if (!Enum.TryParse<PersonType>(row.PersonType, ignoreCase: false, out personType))
                rowErrors.Add(new ImportError(row.LineNumber, "PersonType",
                    $"Invalid PersonType '{row.PersonType}'. Valid values: Natural, Legal, LegalEntity."));

            // ReportFlag
            if (string.IsNullOrWhiteSpace(row.ReportFlag))
                rowErrors.Add(new ImportError(row.LineNumber, "ReportFlag", "ReportFlag is required."));
            else if (!bool.TryParse(row.ReportFlag, out reportFlag))
                rowErrors.Add(new ImportError(row.LineNumber, "ReportFlag", "ReportFlag must be 'true' or 'false'."));

            // Name
            if (string.IsNullOrWhiteSpace(row.Name))
                rowErrors.Add(new ImportError(row.LineNumber, "Name", "Name is required."));
            else if (row.Name.Length > 300)
                rowErrors.Add(new ImportError(row.LineNumber, "Name", "Name must not exceed 300 characters."));

            // Ciiu
            if (string.IsNullOrWhiteSpace(row.Ciiu))
                rowErrors.Add(new ImportError(row.LineNumber, "Ciiu", "Ciiu is required."));
            else if (row.Ciiu.Length > 10)
                rowErrors.Add(new ImportError(row.LineNumber, "Ciiu", "Ciiu must not exceed 10 characters."));

            // Address
            if (string.IsNullOrWhiteSpace(row.Address))
                rowErrors.Add(new ImportError(row.LineNumber, "Address", "Address is required."));
            else if (row.Address.Length > 500)
                rowErrors.Add(new ImportError(row.LineNumber, "Address", "Address must not exceed 500 characters."));

            // DocumentNumber (dedup check)
            if (string.IsNullOrWhiteSpace(row.DocumentNumber))
                rowErrors.Add(new ImportError(row.LineNumber, "DocumentNumber", "DocumentNumber is required."));
            else if (row.DocumentNumber.Length > 50)
                rowErrors.Add(new ImportError(row.LineNumber, "DocumentNumber", "DocumentNumber must not exceed 50 characters."));
            else if (!seenDocumentNumbers.Add(row.DocumentNumber))
                rowErrors.Add(new ImportError(row.LineNumber, "DocumentNumber",
                    $"Duplicate DocumentNumber '{row.DocumentNumber}'."));

            // EntityCode (optional)
            if (row.EntityCode is not null && row.EntityCode.Length > 50)
                rowErrors.Add(new ImportError(row.LineNumber, "EntityCode", "EntityCode must not exceed 50 characters."));

            // InternalLocation
            if (string.IsNullOrWhiteSpace(row.InternalLocation))
                rowErrors.Add(new ImportError(row.LineNumber, "InternalLocation", "InternalLocation is required."));
            else if (row.InternalLocation.Length > 500)
                rowErrors.Add(new ImportError(row.LineNumber, "InternalLocation", "InternalLocation must not exceed 500 characters."));

            // DocumentType FK
            if (string.IsNullOrWhiteSpace(row.DocumentTypeAbbreviation))
                rowErrors.Add(new ImportError(row.LineNumber, "DocumentTypeAbbreviation", "DocumentTypeAbbreviation is required."));
            else if (!docTypeByAbbreviation.TryGetValue(row.DocumentTypeAbbreviation, out documentType))
                rowErrors.Add(new ImportError(row.LineNumber, "DocumentTypeAbbreviation",
                    $"DocumentType '{row.DocumentTypeAbbreviation}' not found."));

            // Country FK
            if (string.IsNullOrWhiteSpace(row.CountryAbbreviation))
                rowErrors.Add(new ImportError(row.LineNumber, "CountryAbbreviation", "CountryAbbreviation is required."));
            else if (!countryIdByAbbreviation.TryGetValue(row.CountryAbbreviation, out var cId))
                rowErrors.Add(new ImportError(row.LineNumber, "CountryAbbreviation",
                    $"Country '{row.CountryAbbreviation}' not found."));
            else
                countryId = cId;

            // Representative FK (optional; resolved from DB + in-batch map)
            if (!string.IsNullOrWhiteSpace(row.RepresentativeDocumentNumber))
            {
                if (!personMap.TryGetValue(row.RepresentativeDocumentNumber, out var repLookup))
                    rowErrors.Add(new ImportError(row.LineNumber, "RepresentativeDocumentNumber",
                        $"Representative with DocumentNumber '{row.RepresentativeDocumentNumber}' not found. " +
                        "Make sure the representative appears earlier in the file or already exists in the database."));
                else
                    representativeId = repLookup.Id;
            }

            // Business rules (only when field resolution succeeded to avoid null refs)
            if (rowErrors.Count == 0 && documentType is not null)
            {
                var ruleErrors = PersonImportRules.Check(
                    documentType,
                    personType,
                    row.DocumentNumber!,
                    representativeId.HasValue);

                foreach (var msg in ruleErrors)
                    rowErrors.Add(new ImportError(row.LineNumber, null, msg));
            }

            // ReportFlag rule (Natural persons cannot be in report)
            if (rowErrors.Count == 0 && personType == PersonType.Natural && reportFlag)
                rowErrors.Add(new ImportError(row.LineNumber, "ReportFlag", "Natural persons cannot be included in the report."));

            if (rowErrors.Count > 0)
            {
                errors.AddRange(rowErrors);
                continue;
            }

            var person = new Person
            {
                Name = row.Name!,
                PersonType = personType,
                Ciiu = row.Ciiu!,
                Address = row.Address!,
                DocumentTypeId = documentType!.Id,
                DocumentNumber = row.DocumentNumber!,
                EntityCode = row.EntityCode,
                RepresentativeId = representativeId,
                ReportFlag = reportFlag,
                CountryId = countryId!.Value,
                InternalLocation = row.InternalLocation!
            };

            staged.Add(person);

            // Immediately update the in-batch map so subsequent rows can reference this person
            personMap[person.DocumentNumber] = new PersonLookup(person.Id, person.PersonType);
        }

        if (request.Mode == ImportMode.Atomic && errors.Count > 0)
            return new ImportResult(0, errors.Count, errors);

        foreach (var person in staged)
            await personRepository.AddAsync(person, cancellationToken);

        if (staged.Count > 0)
            await personRepository.SaveChangesAsync(cancellationToken);

        return new ImportResult(staged.Count, errors.Count, errors);
    }
}

internal record PersonLookup(Guid Id, PersonType PersonType);
