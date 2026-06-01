using equity_regulatory_reporting.Application.Common.Models;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.BoardMembers.Commands.ImportBoardMembers;

public record BoardMemberImportRow(
    int LineNumber,
    string? CompanyDocumentNumber,
    string? MemberDocumentNumber,
    string? PrimaryPositionName,
    string? SecondaryPositionName);

public record ImportBoardMembersCommand(
    IReadOnlyList<BoardMemberImportRow> Rows,
    ImportMode Mode) : IRequest<ImportResult>;
