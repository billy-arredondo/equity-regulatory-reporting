using AutoMapper;
using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Features.DocumentTypes.Dtos;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.DocumentTypes.Queries.GetDocumentTypeById;

public class GetDocumentTypeByIdQueryHandler(IRepository<DocumentType> repository, IMapper mapper)
    : IRequestHandler<GetDocumentTypeByIdQuery, DocumentTypeDto>
{
    public async Task<DocumentTypeDto> Handle(GetDocumentTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var documentType = await repository.Query()
            .Include(d => d.AllowedPersonTypes)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(DocumentType), request.Id);

        return mapper.Map<DocumentTypeDto>(documentType);
    }
}
