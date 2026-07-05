using equity_regulatory_reporting.Application.Common.Models;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.DocumentTypes.Commands.ImportDocumentTypePersonTypes;

public record DocumentTypePersonTypeImportRow(
    int LineNumber,
    string? DocumentTypeAbbreviation,
    string? PersonType);

public record ImportDocumentTypePersonTypesCommand(
    IReadOnlyList<DocumentTypePersonTypeImportRow> Rows,
    ImportMode Mode) : IRequest<ImportResult>;
