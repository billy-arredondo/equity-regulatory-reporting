using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.DocumentTypes.Commands.ImportDocumentTypes;

public class ImportDocumentTypesCommandHandler(IRepository<DocumentType> repository)
    : IRequestHandler<ImportDocumentTypesCommand, ImportResult>
{
    public async Task<ImportResult> Handle(ImportDocumentTypesCommand request, CancellationToken cancellationToken)
    {
        var existingAbbreviations = await repository.Query()
            .Select(d => d.Abbreviation)
            .ToHashSetAsync(StringComparer.OrdinalIgnoreCase, cancellationToken);

        var errors = new List<ImportError>();
        var staged = new List<DocumentType>();
        var seenAbbreviations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in request.Rows)
        {
            var rowErrors = new List<ImportError>();

            if (string.IsNullOrWhiteSpace(row.Name))
                rowErrors.Add(new ImportError(row.LineNumber, "Name", "Name is required."));
            else if (row.Name.Length > 100)
                rowErrors.Add(new ImportError(row.LineNumber, "Name", "Name must not exceed 100 characters."));

            if (string.IsNullOrWhiteSpace(row.Abbreviation))
                rowErrors.Add(new ImportError(row.LineNumber, "Abbreviation", "Abbreviation is required."));
            else if (row.Abbreviation.Length > 10)
                rowErrors.Add(new ImportError(row.LineNumber, "Abbreviation", "Abbreviation must not exceed 10 characters."));
            else if (existingAbbreviations.Contains(row.Abbreviation))
                rowErrors.Add(new ImportError(row.LineNumber, "Abbreviation", $"A document type with abbreviation '{row.Abbreviation}' already exists."));
            else if (!seenAbbreviations.Add(row.Abbreviation))
                rowErrors.Add(new ImportError(row.LineNumber, "Abbreviation", $"Duplicate abbreviation '{row.Abbreviation}' within the file."));

            if (row.ValidationRegex is not null && row.ValidationRegex.Length > 500)
                rowErrors.Add(new ImportError(row.LineNumber, "ValidationRegex", "ValidationRegex must not exceed 500 characters."));

            if (rowErrors.Count > 0)
            {
                errors.AddRange(rowErrors);
                continue;
            }

            staged.Add(new DocumentType
            {
                Name = row.Name!,
                Abbreviation = row.Abbreviation!,
                ValidationRegex = row.ValidationRegex
            });
        }

        if (request.Mode == ImportMode.Atomic && errors.Count > 0)
            return new ImportResult(0, errors.Count, errors);

        foreach (var documentType in staged)
            await repository.AddAsync(documentType, cancellationToken);

        if (staged.Count > 0)
            await repository.SaveChangesAsync(cancellationToken);

        return new ImportResult(staged.Count, errors.Count, errors);
    }
}
