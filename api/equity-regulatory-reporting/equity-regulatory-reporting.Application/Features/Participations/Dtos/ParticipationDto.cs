namespace equity_regulatory_reporting.Application.Features.Participations.Dtos;

public record ParticipationDto(
    Guid Id,
    Guid CompanyId,
    string CompanyName,
    Guid ShareholderId,
    string ShareholderName,
    decimal Percentage,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo);
