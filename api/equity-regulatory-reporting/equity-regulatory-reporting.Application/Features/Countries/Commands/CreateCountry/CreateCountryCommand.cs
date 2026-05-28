using MediatR;

namespace equity_regulatory_reporting.Application.Features.Countries.Commands.CreateCountry;

public record CreateCountryCommand(string Name, string Abbreviation) : IRequest<Guid>;
