using equity_regulatory_reporting.Domain.Enums;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.DocumentTypes.Commands.CreateDocumentType;

public record CreateDocumentTypeCommand(
    string Name,
    string Abbreviation,
    string? ValidationRegex,
    IReadOnlyList<PersonType> AllowedPersonTypes) : IRequest<Guid>;
