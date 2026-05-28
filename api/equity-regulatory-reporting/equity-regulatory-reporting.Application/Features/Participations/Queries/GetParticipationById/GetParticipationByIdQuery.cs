using equity_regulatory_reporting.Application.Features.Participations.Dtos;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Participations.Queries.GetParticipationById;

public record GetParticipationByIdQuery(Guid Id) : IRequest<ParticipationDto>;
