using equity_regulatory_reporting.Application.Features.DocumentTypes.Dtos;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.DocumentTypes.Queries.GetDocumentTypeById;

public record GetDocumentTypeByIdQuery(Guid Id) : IRequest<DocumentTypeDto>;
