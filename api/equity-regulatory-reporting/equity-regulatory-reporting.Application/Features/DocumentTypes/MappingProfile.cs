using AutoMapper;
using equity_regulatory_reporting.Application.Features.DocumentTypes.Dtos;
using equity_regulatory_reporting.Domain.Entities;

namespace equity_regulatory_reporting.Application.Features.DocumentTypes;

public class DocumentTypesMappingProfile : Profile
{
    public DocumentTypesMappingProfile()
    {
        CreateMap<DocumentType, DocumentTypeDto>()
            .ForMember(d => d.AllowedPersonTypes,
                opt => opt.MapFrom(s => s.AllowedPersonTypes.Select(a => a.PersonType).ToList()));
    }
}
