using MediatR;

namespace equity_regulatory_reporting.Application.Features.Positions.Commands.DeletePosition;

public record DeletePositionCommand(Guid Id) : IRequest;
