using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Domain.Entities;
using equity_regulatory_reporting.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.DocumentTypes.Commands.ImportDocumentTypePersonTypes;

public class ImportDocumentTypePersonTypesCommandHandler(IRepository<DocumentType> repository)
    : IRequestHandler<ImportDocumentTypePersonTypesCommand, ImportResult>
{
    public async Task<ImportResult> Handle(
        ImportDocumentTypePersonTypesCommand request,
        CancellationToken cancellationToken)
    {
        var documentTypes = await repository.Query()
            .Include(d => d.AllowedPersonTypes)
            .ToListAsync(cancellationToken);

        var byAbbreviation = documentTypes.ToDictionary(
            d => d.Abbreviation,
            d => d,
            StringComparer.OrdinalIgnoreCase);

        // Seed existing combinations for dedup check
        var seenCombinations = new HashSet<(Guid DocumentTypeId, PersonType PersonType)>(
            documentTypes.SelectMany(d => d.AllowedPersonTypes
                .Select(pt => (d.Id, pt.PersonType))));

        var errors = new List<ImportError>();
        var pending = new List<(DocumentType DocumentType, PersonType PersonType)>();

        foreach (var row in request.Rows)
        {
            var rowErrors = new List<ImportError>();
            DocumentType? documentType = null;
            PersonType personType = default;

            if (string.IsNullOrWhiteSpace(row.DocumentTypeAbbreviation))
                rowErrors.Add(new ImportError(row.LineNumber, "DocumentTypeAbbreviation", "DocumentTypeAbbreviation is required."));
            else if (!byAbbreviation.TryGetValue(row.DocumentTypeAbbreviation, out documentType))
                rowErrors.Add(new ImportError(row.LineNumber, "DocumentTypeAbbreviation", $"DocumentType '{row.DocumentTypeAbbreviation}' not found."));

            if (string.IsNullOrWhiteSpace(row.PersonType))
                rowErrors.Add(new ImportError(row.LineNumber, "PersonType", "PersonType is required."));
            else if (!Enum.TryParse<PersonType>(row.PersonType, ignoreCase: false, out personType))
                rowErrors.Add(new ImportError(row.LineNumber, "PersonType", $"Invalid PersonType '{row.PersonType}'. Valid values: Natural, Legal, LegalEntity."));

            if (rowErrors.Count == 0 && documentType is not null)
            {
                var combination = (documentType.Id, personType);
                if (!seenCombinations.Add(combination))
                    rowErrors.Add(new ImportError(row.LineNumber, null,
                        $"Mapping ({row.DocumentTypeAbbreviation}, {personType}) already exists or is duplicated within the file."));
            }

            if (rowErrors.Count > 0)
            {
                errors.AddRange(rowErrors);
                continue;
            }

            pending.Add((documentType!, personType));
        }

        if (request.Mode == ImportMode.Atomic && errors.Count > 0)
            return new ImportResult(0, errors.Count, errors);

        foreach (var (dt, pt) in pending)
            dt.AllowedPersonTypes.Add(new DocumentTypePersonType { PersonType = pt });

        if (pending.Count > 0)
            await repository.SaveChangesAsync(cancellationToken);

        return new ImportResult(pending.Count, errors.Count, errors);
    }
}
