using equity_regulatory_reporting.Domain.Enums;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Persons.Commands.UpdatePerson;

public record UpdatePersonCommand(
    Guid Id,
    string Name,
    PersonType PersonType,
    string Ciiu,
    string Address,
    Guid DocumentTypeId,
    string DocumentNumber,
    string? EntityCode,
    Guid? RepresentativeId,
    bool ReportFlag,
    Guid CountryId,
    string InternalLocation) : IRequest;
