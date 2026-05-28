using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Persons.Commands.DeletePerson;

public class DeletePersonCommandHandler(IRepository<Person> repository)
    : IRequestHandler<DeletePersonCommand>
{
    public async Task Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {
        var person = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Person), request.Id);

        repository.Remove(person);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
