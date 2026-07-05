using equity_regulatory_reporting.Application.Common.Models;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Countries.Commands.ImportCountries;

public record CountryImportRow(int LineNumber, string? Name, string? Abbreviation);

public record ImportCountriesCommand(IReadOnlyList<CountryImportRow> Rows, ImportMode Mode) : IRequest<ImportResult>;
