using equity_regulatory_reporting.Application.Common.Models;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.DocumentTypes.Commands.ImportDocumentTypes;

public record DocumentTypeImportRow(int LineNumber, string? Name, string? Abbreviation, string? ValidationRegex);

public record ImportDocumentTypesCommand(IReadOnlyList<DocumentTypeImportRow> Rows, ImportMode Mode) : IRequest<ImportResult>;
