using equity_regulatory_reporting.Application.Features.Countries.Dtos;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Countries.Queries.GetCountryById;

public record GetCountryByIdQuery(Guid Id) : IRequest<CountryDto>;
