using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Countries.Commands.CreateCountry;

public class CreateCountryCommandHandler(IRepository<Country> repository)
    : IRequestHandler<CreateCountryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        var country = new Country
        {
            Name = request.Name,
            Abbreviation = request.Abbreviation
        };

        await repository.AddAsync(country, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return country.Id;
    }
}
