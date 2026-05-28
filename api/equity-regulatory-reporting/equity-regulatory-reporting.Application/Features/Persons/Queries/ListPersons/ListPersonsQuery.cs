using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Persons.Dtos;
using equity_regulatory_reporting.Domain.Enums;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Persons.Queries.ListPersons;

public record ListPersonsQuery(PageRequest Page, PersonType? PersonType = null) : IRequest<PagedResult<PersonDto>>;
