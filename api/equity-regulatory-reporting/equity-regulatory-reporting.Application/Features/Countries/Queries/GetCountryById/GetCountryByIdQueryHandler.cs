using AutoMapper;
using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Features.Countries.Dtos;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Countries.Queries.GetCountryById;

public class GetCountryByIdQueryHandler(IRepository<Country> repository, IMapper mapper)
    : IRequestHandler<GetCountryByIdQuery, CountryDto>
{
    public async Task<CountryDto> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
    {
        var country = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Country), request.Id);

        return mapper.Map<CountryDto>(country);
    }
}
