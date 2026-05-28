using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Participations.Commands.DeleteParticipation;

public class DeleteParticipationCommandHandler(IRepository<Participation> repository)
    : IRequestHandler<DeleteParticipationCommand>
{
    public async Task Handle(DeleteParticipationCommand request, CancellationToken cancellationToken)
    {
        var participation = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Participation), request.Id);

        repository.Remove(participation);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
