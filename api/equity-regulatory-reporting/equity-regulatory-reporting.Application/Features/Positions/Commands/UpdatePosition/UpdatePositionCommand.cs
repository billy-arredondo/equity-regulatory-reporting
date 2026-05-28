using MediatR;

namespace equity_regulatory_reporting.Application.Features.Positions.Commands.UpdatePosition;

public record UpdatePositionCommand(Guid Id, string Name) : IRequest;
