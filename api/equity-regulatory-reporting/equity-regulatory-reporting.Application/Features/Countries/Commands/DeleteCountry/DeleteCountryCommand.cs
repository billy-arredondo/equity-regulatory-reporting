using MediatR;

namespace equity_regulatory_reporting.Application.Features.Countries.Commands.DeleteCountry;

public record DeleteCountryCommand(Guid Id) : IRequest;
