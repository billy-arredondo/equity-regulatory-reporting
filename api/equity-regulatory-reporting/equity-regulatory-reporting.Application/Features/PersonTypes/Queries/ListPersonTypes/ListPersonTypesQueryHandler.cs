using equity_regulatory_reporting.Domain.Enums;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.PersonTypes.Queries.ListPersonTypes;

public class ListPersonTypesQueryHandler : IRequestHandler<ListPersonTypesQuery, IReadOnlyList<PersonTypeDto>>
{
    public Task<IReadOnlyList<PersonTypeDto>> Handle(ListPersonTypesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<PersonTypeDto> result = [.. Enum.GetValues<PersonType>().Select(pt => new PersonTypeDto((int)pt, pt.ToString()))];

        return Task.FromResult(result);
    }
}
