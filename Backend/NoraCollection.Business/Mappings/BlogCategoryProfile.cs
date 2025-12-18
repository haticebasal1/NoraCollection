using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.BlogCategoryDtos;

namespace NoraCollection.Business.Mappings;

public class BlogCategoryProfile : Profile
{
    public BlogCategoryProfile()
    {
        CreateMap<BlogCategory, BlogCategoryDto>()
          .ForMember(
            dest => dest.PostCount,
            opt => opt.MapFrom(src => src.BlogPosts.Count))
         .ReverseMap();
        CreateMap<BlogCategoryCreateDto, BlogCategory>();
        CreateMap<BlogCategoryUpdateDto, BlogCategory>();
    }
}
