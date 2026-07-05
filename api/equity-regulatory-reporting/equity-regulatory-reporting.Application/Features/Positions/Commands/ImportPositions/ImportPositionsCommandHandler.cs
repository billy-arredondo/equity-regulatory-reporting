using System.Text.RegularExpressions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.Positions.Commands.ImportPositions;

public class ImportPositionsCommandHandler(IRepository<Position> repository)
    : IRequestHandler<ImportPositionsCommand, ImportResult>
{
    private static readonly Regex ReportCodeRegex =
        new(@"^\d{2}$", RegexOptions.Compiled);

    public async Task<ImportResult> Handle(ImportPositionsCommand request, CancellationToken cancellationToken)
    {
        var existingNames = await repository.Query()
            .Select(p => p.Name)
            .ToHashSetAsync(StringComparer.OrdinalIgnoreCase, cancellationToken);

        var errors = new List<ImportError>();
        var staged = new List<Position>();
        var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in request.Rows)
        {
            var rowErrors = new List<ImportError>();

            if (string.IsNullOrWhiteSpace(row.Name))
                rowErrors.Add(new ImportError(row.LineNumber, "Name", "Name is required."));
            else if (row.Name.Length > 100)
                rowErrors.Add(new ImportError(row.LineNumber, "Name", "Name must not exceed 100 characters."));
            else if (existingNames.Contains(row.Name))
                rowErrors.Add(new ImportError(row.LineNumber, "Name", $"A position named '{row.Name}' already exists."));
            else if (!seenNames.Add(row.Name))
                rowErrors.Add(new ImportError(row.LineNumber, "Name", $"Duplicate position name '{row.Name}' within the file."));

            if (string.IsNullOrWhiteSpace(row.ReportCode))
                rowErrors.Add(new ImportError(row.LineNumber, "ReportCode", "ReportCode is required."));
            else if (!ReportCodeRegex.IsMatch(row.ReportCode))
                rowErrors.Add(new ImportError(row.LineNumber, "ReportCode", "ReportCode must be exactly 2 digits (e.g. '01')."));

            if (rowErrors.Count > 0)
            {
                errors.AddRange(rowErrors);
                continue;
            }

            staged.Add(new Position { Name = row.Name!, ReportCode = row.ReportCode! });
        }

        if (request.Mode == ImportMode.Atomic && errors.Count > 0)
            return new ImportResult(0, errors.Count, errors);

        foreach (var position in staged)
            await repository.AddAsync(position, cancellationToken);

        if (staged.Count > 0)
            await repository.SaveChangesAsync(cancellationToken);

        return new ImportResult(staged.Count, errors.Count, errors);
    }
}
