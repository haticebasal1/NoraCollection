using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.StaticPageDtos;

namespace NoraCollection.Business.Mappings;

public class StaticPageProfile : Profile
{
    public StaticPageProfile()
    {
        CreateMap<StaticPage, StaticPageDto>().ReverseMap();
        CreateMap<StaticPageCreateDto, StaticPage>();
        CreateMap<StaticPageUpdateDto, StaticPage>();
    }
}
