using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Participations.Import;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.Participations.Commands.ImportParticipations;

public class ImportParticipationsCommandHandler(
    IRepository<Participation> repository,
    IRepository<Person> personRepository)
    : IRequestHandler<ImportParticipationsCommand, ImportResult>
{
    public async Task<ImportResult> Handle(ImportParticipationsCommand request, CancellationToken cancellationToken)
    {
        var personMap = await personRepository.Query()
            .ToDictionaryAsync(
                p => p.DocumentNumber,
                p => p,
                StringComparer.OrdinalIgnoreCase,
                cancellationToken);

        var errors = new List<ImportError>();
        var staged = new List<Participation>();

        foreach (var row in request.Rows)
        {
            var rowErrors = new List<ImportError>();
            Person? company = null;
            Person? shareholder = null;
            decimal percentage = 0;
            DateOnly effectiveFrom = default;
            DateOnly? effectiveTo = null;

            // Company FK
            if (string.IsNullOrWhiteSpace(row.CompanyDocumentNumber))
                rowErrors.Add(new ImportError(row.LineNumber, "CompanyDocumentNumber", "CompanyDocumentNumber is required."));
            else if (!personMap.TryGetValue(row.CompanyDocumentNumber, out company))
                rowErrors.Add(new ImportError(row.LineNumber, "CompanyDocumentNumber",
                    $"Person with DocumentNumber '{row.CompanyDocumentNumber}' not found."));

            // Shareholder FK
            if (string.IsNullOrWhiteSpace(row.ShareholderDocumentNumber))
                rowErrors.Add(new ImportError(row.LineNumber, "ShareholderDocumentNumber", "ShareholderDocumentNumber is required."));
            else if (!personMap.TryGetValue(row.ShareholderDocumentNumber, out shareholder))
                rowErrors.Add(new ImportError(row.LineNumber, "ShareholderDocumentNumber",
                    $"Person with DocumentNumber '{row.ShareholderDocumentNumber}' not found."));

            // Percentage
            if (string.IsNullOrWhiteSpace(row.Percentage))
                rowErrors.Add(new ImportError(row.LineNumber, "Percentage", "Percentage is required."));
            else if (!decimal.TryParse(row.Percentage, System.Globalization.NumberStyles.Number,
                         System.Globalization.CultureInfo.InvariantCulture, out percentage))
                rowErrors.Add(new ImportError(row.LineNumber, "Percentage", "Percentage must be a valid decimal number."));
            else if (percentage < 0 || percentage > 100)
                rowErrors.Add(new ImportError(row.LineNumber, "Percentage", "Percentage must be between 0 and 100."));

            // EffectiveFrom
            if (string.IsNullOrWhiteSpace(row.EffectiveFrom))
                rowErrors.Add(new ImportError(row.LineNumber, "EffectiveFrom", "EffectiveFrom is required."));
            else if (!DateOnly.TryParseExact(row.EffectiveFrom, "yyyy-MM-dd", out effectiveFrom))
                rowErrors.Add(new ImportError(row.LineNumber, "EffectiveFrom", "EffectiveFrom must be in yyyy-MM-dd format."));

            // EffectiveTo (optional)
            if (!string.IsNullOrWhiteSpace(row.EffectiveTo))
            {
                if (!DateOnly.TryParseExact(row.EffectiveTo, "yyyy-MM-dd", out var parsedTo))
                    rowErrors.Add(new ImportError(row.LineNumber, "EffectiveTo", "EffectiveTo must be in yyyy-MM-dd format."));
                else if (parsedTo < effectiveFrom)
                    rowErrors.Add(new ImportError(row.LineNumber, "EffectiveTo",
                        "EffectiveTo must be greater than or equal to EffectiveFrom."));
                else
                    effectiveTo = parsedTo;
            }

            // Business rules
            if (rowErrors.Count == 0 && company is not null)
            {
                var ruleErrors = ParticipationImportRules.Check(company);
                foreach (var msg in ruleErrors)
                    rowErrors.Add(new ImportError(row.LineNumber, null, msg));
            }

            if (rowErrors.Count > 0)
            {
                errors.AddRange(rowErrors);
                continue;
            }

            staged.Add(new Participation
            {
                CompanyId = company!.Id,
                ShareholderId = shareholder!.Id,
                Percentage = percentage,
                EffectiveFrom = effectiveFrom,
                EffectiveTo = effectiveTo
            });
        }

        if (request.Mode == ImportMode.Atomic && errors.Count > 0)
            return new ImportResult(0, errors.Count, errors);

        foreach (var participation in staged)
            await repository.AddAsync(participation, cancellationToken);

        if (staged.Count > 0)
            await repository.SaveChangesAsync(cancellationToken);

        return new ImportResult(staged.Count, errors.Count, errors);
    }
}
