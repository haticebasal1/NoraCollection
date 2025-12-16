using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.CustomDesignDtos;

namespace NoraCollection.Business.Mappings;

public class CustomDesignProfile : Profile
{
    public CustomDesignProfile()
    {
        var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
        CreateMap<CustomDesign, CustomDesignDto>()
        .ForMember(
         dest => dest.CreatedAt,
         opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.CreatedAt, turkeyTimeZone)))
         .ForMember(
             dest => dest.UserId,
             opt => opt.MapFrom(src => src.UserId))
             .ForMember(
                 dest => dest.Id,
                 opt => opt.MapFrom(src => src.Id))
                 .ForMember(
                     dest => dest.ReferanceImageUrl,
                     opt => opt.MapFrom(src => src.ReferenceImageUrl))
                     .ReverseMap()
        .ForMember(
            dest => dest.User,
            opt => opt.Ignore())
        .ForMember(
            dest => dest.CreatedAt,
            opt => opt.Ignore())
        .ForMember(
            dest => dest.UpdatedAt,
            opt => opt.Ignore());

        CreateMap<CustomDesignCreateDto, CustomDesign>()
        .ForMember(
            dest => dest.User,
            opt => opt.Ignore())
            .ForMember(
                dest => dest.ReferenceImageUrl,
                opt => opt.MapFrom(src => src.ReferenceImageUrl))
            .ForMember(
                dest => dest.CreatedAt,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.UpdatedAt,
                opt => opt.Ignore());

        CreateMap<CustomDesignUpdateDto,CustomDesign>()
        .ForMember(
            dest=>dest.User,
            opt=>opt.Ignore())
        .ForMember(
            dest=>dest.ReferenceImageUrl,
            opt=>opt.Ignore())
        .ForMember(
            dest=>dest.User,
            opt=>opt.Ignore());
    }
}
