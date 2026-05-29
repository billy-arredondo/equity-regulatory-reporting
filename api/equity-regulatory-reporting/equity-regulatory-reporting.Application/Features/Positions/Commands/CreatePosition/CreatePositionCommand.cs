using MediatR;

namespace equity_regulatory_reporting.Application.Features.Positions.Commands.CreatePosition;

public record CreatePositionCommand(string Name, string ReportCode) : IRequest<Guid>;
