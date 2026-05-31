using AutoMapper;
using equity_regulatory_reporting.Application.Features.Countries.Dtos;
using equity_regulatory_reporting.Domain.Entities;

namespace equity_regulatory_reporting.Application.Features.Countries;

public class CountriesMappingProfile : Profile
{
    public CountriesMappingProfile()
    {
        CreateMap<Country, CountryDto>();
    }
}
