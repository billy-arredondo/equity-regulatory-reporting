using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.DocumentTypes.Commands.CreateDocumentType;

public class CreateDocumentTypeCommandHandler(IRepository<DocumentType> repository)
    : IRequestHandler<CreateDocumentTypeCommand, Guid>
{
    public async Task<Guid> Handle(CreateDocumentTypeCommand request, CancellationToken cancellationToken)
    {
        var documentType = new DocumentType
        {
            Name = request.Name,
            Abbreviation = request.Abbreviation,
            ValidationRegex = request.ValidationRegex,
            AllowedPersonTypes = request.AllowedPersonTypes
                .Select(pt => new DocumentTypePersonType { PersonType = pt })
                .ToList()
        };

        await repository.AddAsync(documentType, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return documentType.Id;
    }
}
