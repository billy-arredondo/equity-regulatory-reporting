using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.DocumentTypes.Commands.DeleteDocumentType;

public class DeleteDocumentTypeCommandHandler(IRepository<DocumentType> repository)
    : IRequestHandler<DeleteDocumentTypeCommand>
{
    public async Task Handle(DeleteDocumentTypeCommand request, CancellationToken cancellationToken)
    {
        var documentType = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(DocumentType), request.Id);

        repository.Remove(documentType);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
