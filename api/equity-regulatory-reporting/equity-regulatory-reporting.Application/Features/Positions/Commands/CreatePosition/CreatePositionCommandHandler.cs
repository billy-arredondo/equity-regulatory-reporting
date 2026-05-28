using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Positions.Commands.CreatePosition;

public class CreatePositionCommandHandler(IRepository<Position> repository)
    : IRequestHandler<CreatePositionCommand, Guid>
{
    public async Task<Guid> Handle(CreatePositionCommand request, CancellationToken cancellationToken)
    {
        var position = new Position { Name = request.Name };

        await repository.AddAsync(position, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return position.Id;
    }
}
