using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using FluentValidation;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Positions.Commands.DeletePosition;

public class DeletePositionCommandHandler(IRepository<Position> repository)
    : IRequestHandler<DeletePositionCommand>
{
    public async Task Handle(DeletePositionCommand request, CancellationToken cancellationToken)
    {
        var position = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Position), request.Id);

        if (position.Name == "No position")
            throw new ValidationException("The 'No position' sentinel record cannot be deleted.");

        repository.Remove(position);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
