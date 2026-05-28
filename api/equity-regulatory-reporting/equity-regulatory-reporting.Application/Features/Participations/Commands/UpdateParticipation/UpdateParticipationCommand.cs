using MediatR;

namespace equity_regulatory_reporting.Application.Features.Participations.Commands.UpdateParticipation;

public record UpdateParticipationCommand(
    Guid Id,
    Guid CompanyId,
    Guid ShareholderId,
    decimal Percentage,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo) : IRequest;
