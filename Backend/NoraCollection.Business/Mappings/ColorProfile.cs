using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.ColorDtos;

namespace NoraCollection.Business.Mappings;

public class ColorProfile : Profile
{
    public ColorProfile()
    {
        CreateMap<Color, ColorDto>().ReverseMap();
        CreateMap<ColorCreateDto, Color>();
        CreateMap<ColorUpdateDto, Color>();
    }
}
