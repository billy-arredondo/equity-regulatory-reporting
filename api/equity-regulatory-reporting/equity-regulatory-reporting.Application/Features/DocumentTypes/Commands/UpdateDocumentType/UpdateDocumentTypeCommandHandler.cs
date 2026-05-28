using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace equity_regulatory_reporting.Application.Features.DocumentTypes.Commands.UpdateDocumentType;

public class UpdateDocumentTypeCommandHandler(IRepository<DocumentType> repository)
    : IRequestHandler<UpdateDocumentTypeCommand>
{
    public async Task Handle(UpdateDocumentTypeCommand request, CancellationToken cancellationToken)
    {
        var documentType = await repository.Query()
            .Include(d => d.AllowedPersonTypes)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(DocumentType), request.Id);

        documentType.Name = request.Name;
        documentType.Abbreviation = request.Abbreviation;
        documentType.ValidationRegex = request.ValidationRegex;

        documentType.AllowedPersonTypes.Clear();
        foreach (var personType in request.AllowedPersonTypes)
            documentType.AllowedPersonTypes.Add(new DocumentTypePersonType { PersonType = personType });

        repository.Update(documentType);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
