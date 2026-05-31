using AutoMapper;
using equity_regulatory_reporting.Application.Features.Positions.Dtos;
using equity_regulatory_reporting.Domain.Entities;

namespace equity_regulatory_reporting.Application.Features.Positions;

public class PositionsMappingProfile : Profile
{
    public PositionsMappingProfile()
    {
        CreateMap<Position, PositionDto>();
    }
}
