using MediatR;

namespace equity_regulatory_reporting.Application.Features.BoardMembers.Commands.DeleteBoardMember;

public record DeleteBoardMemberCommand(Guid Id) : IRequest;
