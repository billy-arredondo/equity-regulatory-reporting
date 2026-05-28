using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Countries.Dtos;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Countries.Queries.ListCountries;

public record ListCountriesQuery(PageRequest Page) : IRequest<PagedResult<CountryDto>>;
