using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.ColorDtos;

namespace NoraCollection.Business.Mappings;

public class ColorProfile : Profile
{
    public ColorProfile()
    {
        CreateMap<Color, ColorDto>();
        CreateMap<ColorCreateDto, Color>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
               .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
               .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
               .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
               .ForMember(dest => dest.HexCode, opt => opt.MapFrom(src => 
                   string.IsNullOrWhiteSpace(src.HexCode) 
                       ? src.HexCode 
                       : (src.HexCode.Trim().StartsWith("#") 
                           ? src.HexCode.Trim().ToUpper() 
                           : $"#{src.HexCode.Trim().ToUpper()}")));
        CreateMap<ColorUpdateDto, Color>()
                       .ForMember(dest => dest.Id, opt => opt.Ignore())
                       .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                       .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                       .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                       .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                       .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                       .ForMember(dest => dest.DisplayOrder, opt => opt.Ignore())
                       .ForMember(dest => dest.HexCode, opt => opt.MapFrom(src => 
                           string.IsNullOrWhiteSpace(src.HexCode) 
                               ? src.HexCode 
                               : (src.HexCode.Trim().StartsWith("#") 
                                   ? src.HexCode.Trim().ToUpper() 
                                   : $"#{src.HexCode.Trim().ToUpper()}")));
    }
}
