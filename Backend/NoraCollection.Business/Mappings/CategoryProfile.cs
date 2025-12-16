using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.CategoryDtos;

namespace NoraCollection.Business.Mappings;

public class CategoryProfile:Profile
{
 public CategoryProfile()
    {
        var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
        CreateMap<Category, CategoryDto>()
         .ForMember(
            dest=>dest.CreatedAt,
            opt=>opt.MapFrom(src=>TimeZoneInfo.ConvertTime(src.CreatedAt.UtcDateTime,turkeyTimeZone)))
            .ForMember(
                dest=>dest.UpdatedAt,
                opt=>opt.MapFrom(src=>TimeZoneInfo.ConvertTime(src.UpdatedAt.UtcDateTime,turkeyTimeZone)))
                .ForMember(
                    dest=>dest.DeletedAt,
                    opt=>opt.MapFrom(src=>TimeZoneInfo.ConvertTime(src.DeletedAt.UtcDateTime,turkeyTimeZone)));

        CreateMap<CategoryCreateDto,Category>();
        CreateMap<CategoryUpdateDto,Category>();
}
}
