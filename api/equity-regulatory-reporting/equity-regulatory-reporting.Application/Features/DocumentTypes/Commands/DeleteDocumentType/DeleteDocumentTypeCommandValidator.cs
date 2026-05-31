using FluentValidation;

namespace equity_regulatory_reporting.Application.Features.DocumentTypes.Commands.DeleteDocumentType;

public class DeleteDocumentTypeCommandValidator : AbstractValidator<DeleteDocumentTypeCommand>
{
    public DeleteDocumentTypeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
