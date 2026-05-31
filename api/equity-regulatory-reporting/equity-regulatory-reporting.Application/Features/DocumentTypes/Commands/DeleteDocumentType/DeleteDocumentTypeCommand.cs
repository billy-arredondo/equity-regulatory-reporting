using MediatR;

namespace equity_regulatory_reporting.Application.Features.DocumentTypes.Commands.DeleteDocumentType;

public record DeleteDocumentTypeCommand(Guid Id) : IRequest;
