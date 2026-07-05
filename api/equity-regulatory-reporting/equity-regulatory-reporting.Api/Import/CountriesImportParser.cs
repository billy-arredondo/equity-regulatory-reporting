using equity_regulatory_reporting.Application.Common.Csv;
using equity_regulatory_reporting.Application.Features.Countries.Commands.ImportCountries;

namespace equity_regulatory_reporting.Api.Import;

public static class CountriesImportParser
{
    public static IReadOnlyList<CountryImportRow> Parse(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var doc = PsvParser.Parse(stream, ["Name", "Abbreviation"]);
        return doc.Rows
            .Select(r => new CountryImportRow(r.LineNumber, doc.Get(r, "Name"), doc.Get(r, "Abbreviation")))
            .ToList();
    }
}
