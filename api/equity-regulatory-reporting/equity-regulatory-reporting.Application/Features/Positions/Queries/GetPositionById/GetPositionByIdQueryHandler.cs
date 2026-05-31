using AutoMapper;
using equity_regulatory_reporting.Application.Common.Exceptions;
using equity_regulatory_reporting.Application.Common.Interfaces;
using equity_regulatory_reporting.Application.Features.Positions.Dtos;
using equity_regulatory_reporting.Domain.Entities;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Positions.Queries.GetPositionById;

public class GetPositionByIdQueryHandler(IRepository<Position> repository, IMapper mapper)
    : IRequestHandler<GetPositionByIdQuery, PositionDto>
{
    public async Task<PositionDto> Handle(GetPositionByIdQuery request, CancellationToken cancellationToken)
    {
        var position = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Position), request.Id);

        return mapper.Map<PositionDto>(position);
    }
}
