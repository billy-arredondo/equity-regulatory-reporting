using equity_regulatory_reporting.Application.Common.Csv;
using equity_regulatory_reporting.Application.Features.Positions.Commands.ImportPositions;

namespace equity_regulatory_reporting.Api.Import;

public static class PositionsImportParser
{
    public static IReadOnlyList<PositionImportRow> Parse(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var doc = PsvParser.Parse(stream, ["Name", "ReportCode"]);
        return doc.Rows
            .Select(r => new PositionImportRow(r.LineNumber, doc.Get(r, "Name"), doc.Get(r, "ReportCode")))
            .ToList();
    }
}
