using equity_regulatory_reporting.Application.Common.Csv;
using equity_regulatory_reporting.Application.Features.DocumentTypes.Commands.ImportDocumentTypePersonTypes;
using equity_regulatory_reporting.Application.Features.DocumentTypes.Commands.ImportDocumentTypes;

namespace equity_regulatory_reporting.Api.Import;

public static class DocumentTypesImportParser
{
    public static IReadOnlyList<DocumentTypeImportRow> ParseDocumentTypes(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var doc = PsvParser.Parse(stream, ["Name", "Abbreviation", "ValidationRegex"]);
        return doc.Rows
            .Select(r => new DocumentTypeImportRow(
                r.LineNumber,
                doc.Get(r, "Name"),
                doc.Get(r, "Abbreviation"),
                doc.Get(r, "ValidationRegex")))
            .ToList();
    }

    public static IReadOnlyList<DocumentTypePersonTypeImportRow> ParsePersonTypeMappings(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var doc = PsvParser.Parse(stream, ["DocumentTypeAbbreviation", "PersonType"]);
        return doc.Rows
            .Select(r => new DocumentTypePersonTypeImportRow(
                r.LineNumber,
                doc.Get(r, "DocumentTypeAbbreviation"),
                doc.Get(r, "PersonType")))
            .ToList();
    }
}
