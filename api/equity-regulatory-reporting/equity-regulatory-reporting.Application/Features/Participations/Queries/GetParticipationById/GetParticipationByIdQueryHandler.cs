using AutoMapper;
using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Features.Participations.Dtos;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.Participations.Queries.GetParticipationById;

public class GetParticipationByIdQueryHandler(IRepository<Participation> repository, IMapper mapper)
    : IRequestHandler<GetParticipationByIdQuery, ParticipationDto>
{
    public async Task<ParticipationDto> Handle(GetParticipationByIdQuery request, CancellationToken cancellationToken)
    {
        var participation = await repository.Query()
            .Include(p => p.Company)
            .Include(p => p.Shareholder)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Participation), request.Id);

        return mapper.Map<ParticipationDto>(participation);
    }
}
