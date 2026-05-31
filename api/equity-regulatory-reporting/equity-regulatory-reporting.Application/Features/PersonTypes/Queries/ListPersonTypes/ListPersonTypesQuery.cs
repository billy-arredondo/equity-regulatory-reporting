using MediatR;

namespace equity_regulatory_reporting.Application.Features.PersonTypes.Queries.ListPersonTypes;

public record PersonTypeDto(int Id, string Name);

public record ListPersonTypesQuery : IRequest<IReadOnlyList<PersonTypeDto>>;
