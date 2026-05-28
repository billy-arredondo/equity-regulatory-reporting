using equity_regulatory_reporting.Domain.Enums;

namespace equity_regulatory_reporting.Application.Features.DocumentTypes.Dtos;

public record DocumentTypeDto(
    Guid Id,
    string Name,
    string Abbreviation,
    string? ValidationRegex,
    IReadOnlyList<PersonType> AllowedPersonTypes);
