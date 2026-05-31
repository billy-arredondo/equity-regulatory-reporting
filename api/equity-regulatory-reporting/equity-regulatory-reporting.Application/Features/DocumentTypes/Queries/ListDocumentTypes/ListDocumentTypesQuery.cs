using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.DocumentTypes.Dtos;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.DocumentTypes.Queries.ListDocumentTypes;

public record ListDocumentTypesQuery(PageRequest Page) : IRequest<PagedResult<DocumentTypeDto>>;
