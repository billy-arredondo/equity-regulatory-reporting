using AutoMapper;
using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Features.Persons.Dtos;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.Persons.Queries.GetPersonById;

public class GetPersonByIdQueryHandler(IRepository<Person> repository, IMapper mapper)
    : IRequestHandler<GetPersonByIdQuery, PersonDetailDto>
{
    public async Task<PersonDetailDto> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var person = await repository.Query()
            .Include(p => p.DocumentType)
            .Include(p => p.Country)
            .Include(p => p.Representative)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Person), request.Id);

        return mapper.Map<PersonDetailDto>(person);
    }
}
