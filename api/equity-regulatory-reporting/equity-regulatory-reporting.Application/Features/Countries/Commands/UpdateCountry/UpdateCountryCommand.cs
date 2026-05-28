using MediatR;

namespace equity_regulatory_reporting.Application.Features.Countries.Commands.UpdateCountry;

public record UpdateCountryCommand(Guid Id, string Name, string Abbreviation) : IRequest;
