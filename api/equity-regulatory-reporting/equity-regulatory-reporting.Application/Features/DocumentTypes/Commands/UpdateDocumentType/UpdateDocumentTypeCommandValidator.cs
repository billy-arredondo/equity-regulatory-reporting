using FluentValidation;

namespace equity_regulatory_reporting.Application.Features.DocumentTypes.Commands.UpdateDocumentType;

public class UpdateDocumentTypeCommandValidator : AbstractValidator<UpdateDocumentTypeCommand>
{
    public UpdateDocumentTypeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Abbreviation).NotEmpty().MaximumLength(10);
        RuleFor(x => x.ValidationRegex).MaximumLength(500).When(x => x.ValidationRegex is not null);
        RuleFor(x => x.AllowedPersonTypes).NotNull();
    }
}
