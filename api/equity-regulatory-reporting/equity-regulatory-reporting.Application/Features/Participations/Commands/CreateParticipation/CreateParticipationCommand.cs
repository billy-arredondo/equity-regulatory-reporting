using MediatR;

namespace equity_regulatory_reporting.Application.Features.Participations.Commands.CreateParticipation;

public record CreateParticipationCommand(
    Guid CompanyId,
    Guid ShareholderId,
    decimal Percentage,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo) : IRequest<Guid>;
