using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.HeroBannerDtos;

namespace NoraCollection.Business.Mappings;

public class HeroBannerProfile : Profile
{
    public HeroBannerProfile()
    {
        CreateMap<HeroBanner, HeroBannerDto>().ReverseMap();
        CreateMap<HeroBannerCreateDto, HeroBanner>();
        CreateMap<HeroBannerUpdateDto, HeroBanner>();
    }
}
