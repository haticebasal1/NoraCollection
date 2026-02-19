using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.FavoriteDtos;

namespace NoraCollection.Business.Mappings;

public class FavoriteProfile : Profile
{
    public FavoriteProfile()
    {
        var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
        CreateMap<Favorite, FavoriteDto>()
        .ForMember(
            dest => dest.CreatedDate,
            opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.CreatedAt.UtcDateTime, turkeyTimeZone)))
            .ForMember(
                dest => dest.UpdatedDate,
                opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.UpdatedAt.UtcDateTime, turkeyTimeZone)))
                .ReverseMap();
        CreateMap<FavoriteCreateDto, Favorite>();
    }
}
