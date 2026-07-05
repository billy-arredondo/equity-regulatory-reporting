using equity_regulatory_reporting.Application.Common.Csv;
using equity_regulatory_reporting.Application.Features.Participations.Commands.ImportParticipations;

namespace equity_regulatory_reporting.Api.Import;

public static class ParticipationsImportParser
{
    public static IReadOnlyList<ParticipationImportRow> Parse(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var doc = PsvParser.Parse(stream,
            ["CompanyDocumentNumber", "ShareholderDocumentNumber", "Percentage", "EffectiveFrom", "EffectiveTo"]);
        return doc.Rows
            .Select(r => new ParticipationImportRow(
                r.LineNumber,
                doc.Get(r, "CompanyDocumentNumber"),
                doc.Get(r, "ShareholderDocumentNumber"),
                doc.Get(r, "Percentage"),
                doc.Get(r, "EffectiveFrom"),
                doc.Get(r, "EffectiveTo")))
            .ToList();
    }
}
