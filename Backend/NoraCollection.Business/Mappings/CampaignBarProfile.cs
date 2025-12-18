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
        CreateMap<CampaignBarCreateDto, CampaignBar>();
        CreateMap<CampaignBarUpdateDto, CampaignBar>();
    }
}
