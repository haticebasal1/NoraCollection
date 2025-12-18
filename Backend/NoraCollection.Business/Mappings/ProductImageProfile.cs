using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.ProductImageDtos;

namespace NoraCollection.Business.Mappings;

public class ProductImageProfile : Profile
{
    public ProductImageProfile()
    {
        CreateMap<ProductImage, ProductImageDto>().ReverseMap();
        CreateMap<ProductImageCreateDto, ProductImage>();
        CreateMap<ProductImageUpdateDto, ProductImage>();
    }
}
