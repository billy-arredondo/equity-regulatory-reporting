using MediatR;

namespace equity_regulatory_reporting.Application.Features.Persons.Commands.DeletePerson;

public record DeletePersonCommand(Guid Id) : IRequest;
