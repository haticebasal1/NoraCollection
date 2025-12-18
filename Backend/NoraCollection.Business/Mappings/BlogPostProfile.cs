using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.BlogPostDtos;

namespace NoraCollection.Business.Mappings;

public class BlogPostProfile:Profile
{
   public BlogPostProfile()
    {
        CreateMap<BlogPost,BlogPostDto>().ReverseMap();
        CreateMap<BlogPostCreateDto,BlogPost>();
        CreateMap<BlogPostUpdateDto,BlogPost>();
    }
}
