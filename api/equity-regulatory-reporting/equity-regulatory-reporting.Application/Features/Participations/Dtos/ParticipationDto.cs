using equity_regulatory_reporting.Domain.Enums;

namespace equity_regulatory_reporting.Application.Features.Participations.Dtos;

public record ParticipationDto(
    Guid Id,
    Guid CompanyId,
    string CompanyName,
    PersonType CompanyPersonType,
    Guid ShareholderId,
    string ShareholderName,
    PersonType ShareholderPersonType,
    decimal Percentage,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo);
