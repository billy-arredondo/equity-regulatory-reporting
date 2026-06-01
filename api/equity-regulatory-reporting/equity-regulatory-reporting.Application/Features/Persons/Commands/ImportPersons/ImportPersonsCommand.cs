using equity_regulatory_reporting.Application.Common.Models;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Persons.Commands.ImportPersons;

public record PersonImportRow(
    int LineNumber,
    string? Name,
    string? PersonType,
    string? Ciiu,
    string? Address,
    string? DocumentTypeAbbreviation,
    string? DocumentNumber,
    string? EntityCode,
    string? RepresentativeDocumentNumber,
    string? ReportFlag,
    string? CountryAbbreviation,
    string? InternalLocation);

public record ImportPersonsCommand(IReadOnlyList<PersonImportRow> Rows, ImportMode Mode) : IRequest<ImportResult>;
