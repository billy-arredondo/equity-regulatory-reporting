using equity_regulatory_reporting.Application.Common.Csv;
using equity_regulatory_reporting.Application.Features.BoardMembers.Commands.ImportBoardMembers;

namespace equity_regulatory_reporting.Api.Import;

public static class BoardMembersImportParser
{
    public static IReadOnlyList<BoardMemberImportRow> Parse(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var doc = PsvParser.Parse(stream,
            ["CompanyDocumentNumber", "MemberDocumentNumber", "PrimaryPositionName", "SecondaryPositionName"]);
        return doc.Rows
            .Select(r => new BoardMemberImportRow(
                r.LineNumber,
                doc.Get(r, "CompanyDocumentNumber"),
                doc.Get(r, "MemberDocumentNumber"),
                doc.Get(r, "PrimaryPositionName"),
                doc.Get(r, "SecondaryPositionName")))
            .ToList();
    }
}
