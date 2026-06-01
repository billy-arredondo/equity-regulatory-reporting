using equity_regulatory_reporting.Application.Common.Models;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Participations.Commands.ImportParticipations;

public record ParticipationImportRow(
    int LineNumber,
    string? CompanyDocumentNumber,
    string? ShareholderDocumentNumber,
    string? Percentage,
    string? EffectiveFrom,
    string? EffectiveTo);

public record ImportParticipationsCommand(
    IReadOnlyList<ParticipationImportRow> Rows,
    ImportMode Mode) : IRequest<ImportResult>;
