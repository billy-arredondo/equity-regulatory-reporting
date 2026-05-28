using AutoMapper;
using equity_regulatory_reporting.Application.Features.BoardMembers.Dtos;
using equity_regulatory_reporting.Domain.Entities;

namespace equity_regulatory_reporting.Application.Features.BoardMembers;

public class BoardMembersMappingProfile : Profile
{
    public BoardMembersMappingProfile()
    {
        CreateMap<BoardMember, BoardMemberDto>()
            .ForMember(d => d.CompanyName, opt => opt.MapFrom(s => s.Company.Name))
            .ForMember(d => d.MemberName, opt => opt.MapFrom(s => s.Member.Name))
            .ForMember(d => d.PrimaryPositionName, opt => opt.MapFrom(s => s.PrimaryPosition.Name))
            .ForMember(d => d.SecondaryPositionName, opt => opt.MapFrom(s => s.SecondaryPosition.Name));
    }
}
