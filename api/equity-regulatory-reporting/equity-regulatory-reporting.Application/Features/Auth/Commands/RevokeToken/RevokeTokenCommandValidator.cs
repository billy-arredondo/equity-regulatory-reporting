using FluentValidation;

namespace equity_regulatory_reporting.Application.Features.Auth.Commands.RevokeToken;

public class RevokeTokenCommandValidator : AbstractValidator<RevokeTokenCommand>
{
    public RevokeTokenCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
    }
}
