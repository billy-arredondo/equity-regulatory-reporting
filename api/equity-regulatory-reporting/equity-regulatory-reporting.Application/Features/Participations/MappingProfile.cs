using AutoMapper;
using equity_regulatory_reporting.Application.Features.Participations.Dtos;
using equity_regulatory_reporting.Domain.Entities;

namespace equity_regulatory_reporting.Application.Features.Participations;

public class ParticipationsMappingProfile : Profile
{
    public ParticipationsMappingProfile()
    {
        CreateMap<Participation, ParticipationDto>()
            .ForMember(d => d.CompanyName, opt => opt.MapFrom(s => s.Company.Name))
            .ForMember(d => d.ShareholderName, opt => opt.MapFrom(s => s.Shareholder.Name));
    }
}
