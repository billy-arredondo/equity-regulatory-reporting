using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Countries.Commands.UpdateCountry;

public class UpdateCountryCommandHandler(IRepository<Country> repository)
    : IRequestHandler<UpdateCountryCommand>
{
    public async Task Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        var country = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Country), request.Id);

        country.Name = request.Name;
        country.Abbreviation = request.Abbreviation;

        repository.Update(country);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
