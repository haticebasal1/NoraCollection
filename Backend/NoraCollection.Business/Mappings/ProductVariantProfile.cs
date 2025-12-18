using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.ProductVariantDtos;

namespace NoraCollection.Business.Mappings;

public class ProductVariantProfile : Profile
{
    public ProductVariantProfile()
    {
        CreateMap<ProductVariant, ProductVariantDto>().ReverseMap();
        CreateMap<ProductVariantCreateDto, ProductVariant>();
        CreateMap<ProductVariantUpdateDto, ProductVariant>();
    }
}
