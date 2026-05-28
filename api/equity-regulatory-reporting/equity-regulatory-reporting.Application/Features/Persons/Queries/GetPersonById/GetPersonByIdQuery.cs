using equity_regulatory_reporting.Application.Features.Persons.Dtos;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Persons.Queries.GetPersonById;

public record GetPersonByIdQuery(Guid Id) : IRequest<PersonDetailDto>;
