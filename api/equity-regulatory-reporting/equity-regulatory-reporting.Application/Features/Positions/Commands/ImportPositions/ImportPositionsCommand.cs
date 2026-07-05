using equity_regulatory_reporting.Application.Common.Models;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Positions.Commands.ImportPositions;

public record PositionImportRow(int LineNumber, string? Name, string? ReportCode);

public record ImportPositionsCommand(IReadOnlyList<PositionImportRow> Rows, ImportMode Mode) : IRequest<ImportResult>;
