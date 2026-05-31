using AutoMapper;
using equity_regulatory_reporting.Application.Features.Persons.Dtos;
using equity_regulatory_reporting.Domain.Entities;

namespace equity_regulatory_reporting.Application.Features.Persons;

public class PersonsMappingProfile : Profile
{
    public PersonsMappingProfile()
    {
        CreateMap<Person, PersonDto>()
            .ForMember(d => d.DocumentTypeName, opt => opt.MapFrom(s => s.DocumentType.Name))
            .ForMember(d => d.CountryName, opt => opt.MapFrom(s => s.Country.Name));

        CreateMap<Person, PersonDetailDto>()
            .ForMember(d => d.DocumentTypeName, opt => opt.MapFrom(s => s.DocumentType.Name))
            .ForMember(d => d.CountryName, opt => opt.MapFrom(s => s.Country.Name))
            .ForMember(d => d.RepresentativeName, opt => opt.MapFrom(s => s.Representative != null ? s.Representative.Name : null));
    }
}
