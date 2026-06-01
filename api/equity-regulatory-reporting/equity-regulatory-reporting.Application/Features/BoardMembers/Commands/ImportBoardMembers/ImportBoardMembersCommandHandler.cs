using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.BoardMembers.Import;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.BoardMembers.Commands.ImportBoardMembers;

public class ImportBoardMembersCommandHandler(
    IRepository<BoardMember> repository,
    IRepository<Person> personRepository,
    IRepository<Position> positionRepository)
    : IRequestHandler<ImportBoardMembersCommand, ImportResult>
{
    private const string NoPositionName = "No position";

    public async Task<ImportResult> Handle(ImportBoardMembersCommand request, CancellationToken cancellationToken)
    {
        var personMap = await personRepository.Query()
            .ToDictionaryAsync(
                p => p.DocumentNumber,
                p => p,
                StringComparer.OrdinalIgnoreCase,
                cancellationToken);

        var positionMap = await positionRepository.Query()
            .ToDictionaryAsync(
                p => p.Name,
                p => p.Id,
                StringComparer.OrdinalIgnoreCase,
                cancellationToken);

        // Validate the sentinel exists up front
        if (!positionMap.TryGetValue(NoPositionName, out var noPositionId))
        {
            throw new FluentValidation.ValidationException(
                $"The '{NoPositionName}' sentinel record is missing from the database.");
        }

        var errors = new List<ImportError>();
        var staged = new List<BoardMember>();

        foreach (var row in request.Rows)
        {
            var rowErrors = new List<ImportError>();
            Person? company = null;
            Person? member = null;
            Guid primaryPositionId = default;
            Guid secondaryPositionId = noPositionId;

            // Company FK
            if (string.IsNullOrWhiteSpace(row.CompanyDocumentNumber))
                rowErrors.Add(new ImportError(row.LineNumber, "CompanyDocumentNumber", "CompanyDocumentNumber is required."));
            else if (!personMap.TryGetValue(row.CompanyDocumentNumber, out company))
                rowErrors.Add(new ImportError(row.LineNumber, "CompanyDocumentNumber",
                    $"Person with DocumentNumber '{row.CompanyDocumentNumber}' not found."));

            // Member FK
            if (string.IsNullOrWhiteSpace(row.MemberDocumentNumber))
                rowErrors.Add(new ImportError(row.LineNumber, "MemberDocumentNumber", "MemberDocumentNumber is required."));
            else if (!personMap.TryGetValue(row.MemberDocumentNumber, out member))
                rowErrors.Add(new ImportError(row.LineNumber, "MemberDocumentNumber",
                    $"Person with DocumentNumber '{row.MemberDocumentNumber}' not found."));

            // PrimaryPosition FK
            if (string.IsNullOrWhiteSpace(row.PrimaryPositionName))
                rowErrors.Add(new ImportError(row.LineNumber, "PrimaryPositionName", "PrimaryPositionName is required."));
            else if (!positionMap.TryGetValue(row.PrimaryPositionName, out primaryPositionId))
                rowErrors.Add(new ImportError(row.LineNumber, "PrimaryPositionName",
                    $"Position '{row.PrimaryPositionName}' not found."));

            // SecondaryPosition FK (optional; defaults to "No position" sentinel when empty)
            if (!string.IsNullOrWhiteSpace(row.SecondaryPositionName))
            {
                if (!positionMap.TryGetValue(row.SecondaryPositionName, out secondaryPositionId))
                    rowErrors.Add(new ImportError(row.LineNumber, "SecondaryPositionName",
                        $"Position '{row.SecondaryPositionName}' not found."));
            }

            // Business rules
            if (rowErrors.Count == 0 && company is not null && member is not null)
            {
                var ruleErrors = BoardMemberImportRules.Check(company, member);
                foreach (var msg in ruleErrors)
                    rowErrors.Add(new ImportError(row.LineNumber, null, msg));
            }

            if (rowErrors.Count > 0)
            {
                errors.AddRange(rowErrors);
                continue;
            }

            staged.Add(new BoardMember
            {
                CompanyId = company!.Id,
                MemberId = member!.Id,
                PrimaryPositionId = primaryPositionId,
                SecondaryPositionId = secondaryPositionId
            });
        }

        if (request.Mode == ImportMode.Atomic && errors.Count > 0)
            return new ImportResult(0, errors.Count, errors);

        foreach (var boardMember in staged)
            await repository.AddAsync(boardMember, cancellationToken);

        if (staged.Count > 0)
            await repository.SaveChangesAsync(cancellationToken);

        return new ImportResult(staged.Count, errors.Count, errors);
    }
}
