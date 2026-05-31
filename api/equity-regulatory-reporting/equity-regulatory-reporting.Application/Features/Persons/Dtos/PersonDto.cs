using equity_regulatory_reporting.Domain.Enums;

namespace equity_regulatory_reporting.Application.Features.Persons.Dtos;

public record PersonDto(
    Guid Id,
    string Name,
    PersonType PersonType,
    string DocumentTypeName,
    string DocumentNumber,
    string CountryName,
    bool ReportFlag);

public record PersonDetailDto(
    Guid Id,
    string Name,
    PersonType PersonType,
    string Ciiu,
    string Address,
    Guid DocumentTypeId,
    string DocumentTypeName,
    string DocumentNumber,
    string? EntityCode,
    Guid? RepresentativeId,
    string? RepresentativeName,
    bool ReportFlag,
    Guid CountryId,
    string CountryName,
    string InternalLocation);
