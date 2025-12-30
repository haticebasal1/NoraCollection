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
        
        CreateMap<ProductImageCreateDto, ProductImage>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()) // ImageUrl manuel olarak set ediliyor
            .ForMember(dest => dest.Product, opt => opt.Ignore()) // Navigation property ignore
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id otomatik oluşturuluyor
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        
        CreateMap<ProductImageUpdateDto, ProductImage>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()) // ImageUrl manuel olarak set ediliyor
            .ForMember(dest => dest.Product, opt => opt.Ignore()) // Navigation property ignore
            .ForMember(dest => dest.ProductId, opt => opt.Ignore()) // ProductId değiştirilmemeli
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}
