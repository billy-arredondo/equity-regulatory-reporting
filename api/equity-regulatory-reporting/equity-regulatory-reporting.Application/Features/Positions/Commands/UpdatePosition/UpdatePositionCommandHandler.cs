using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using FluentValidation;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Positions.Commands.UpdatePosition;

public class UpdatePositionCommandHandler(IRepository<Position> repository)
    : IRequestHandler<UpdatePositionCommand>
{
    public async Task Handle(UpdatePositionCommand request, CancellationToken cancellationToken)
    {
        var position = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Position), request.Id);

        if (position.Name == "No position")
            throw new ValidationException("The 'No position' sentinel record cannot be renamed.");

        position.Name = request.Name;

        repository.Update(position);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
