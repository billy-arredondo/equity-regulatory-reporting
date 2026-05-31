using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Countries.Commands.DeleteCountry;

public class DeleteCountryCommandHandler(IRepository<Country> repository)
    : IRequestHandler<DeleteCountryCommand>
{
    public async Task Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
    {
        var country = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Country), request.Id);

        repository.Remove(country);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
