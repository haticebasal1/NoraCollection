using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.CampaignBarDtos;

namespace NoraCollection.Business.Mappings;

public class CampaignBarProfile : Profile
{
    public CampaignBarProfile()
    {
        CreateMap<CampaignBar, CampaignBarDto>().ReverseMap();
        CreateMap<CampaignBarCreateDto, CampaignBar>()
          .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
          .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.DisplayOrder));
        CreateMap<CampaignBarUpdateDto, CampaignBar>();
    }
}
