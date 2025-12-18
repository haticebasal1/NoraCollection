using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.SiteSettingDtos;

namespace NoraCollection.Business.Mappings;

public class SiteSettingProfile : Profile
{
    public SiteSettingProfile()
    {
        CreateMap<SiteSetting, SiteSettingDto>().ReverseMap();
        CreateMap<SiteSettingCreateDto, SiteSetting>();
        CreateMap<SiteSettingUpdateDto, SiteSetting>();
    }
}
