using MediatR;

namespace equity_regulatory_reporting.Application.Features.Participations.Commands.DeleteParticipation;

public record DeleteParticipationCommand(Guid Id) : IRequest;
