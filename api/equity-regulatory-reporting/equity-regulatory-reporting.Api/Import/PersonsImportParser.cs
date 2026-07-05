using equity_regulatory_reporting.Application.Common.Csv;
using equity_regulatory_reporting.Application.Features.Persons.Commands.ImportPersons;

namespace equity_regulatory_reporting.Api.Import;

public static class PersonsImportParser
{
    private static readonly string[] Columns =
    [
        "Name", "PersonType", "Ciiu", "Address",
        "DocumentTypeAbbreviation", "DocumentNumber",
        "EntityCode", "RepresentativeDocumentNumber",
        "ReportFlag", "CountryAbbreviation", "InternalLocation"
    ];

    public static IReadOnlyList<PersonImportRow> Parse(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var doc = PsvParser.Parse(stream, Columns);
        return doc.Rows
            .Select(r => new PersonImportRow(
                r.LineNumber,
                doc.Get(r, "Name"),
                doc.Get(r, "PersonType"),
                doc.Get(r, "Ciiu"),
                doc.Get(r, "Address"),
                doc.Get(r, "DocumentTypeAbbreviation"),
                doc.Get(r, "DocumentNumber"),
                doc.Get(r, "EntityCode"),
                doc.Get(r, "RepresentativeDocumentNumber"),
                doc.Get(r, "ReportFlag"),
                doc.Get(r, "CountryAbbreviation"),
                doc.Get(r, "InternalLocation")))
            .ToList();
    }
}
