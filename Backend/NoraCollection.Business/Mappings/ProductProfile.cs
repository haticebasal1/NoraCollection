using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.ProductDtos;

namespace NoraCollection.Business.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
        CreateMap<Product, ProductDto>()
         .ForMember(
            dest => dest.CreatedAt,
            opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.CreatedAt.UtcDateTime, turkeyTimeZone)))
            .ForMember(
                dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.UpdatedAt.UtcDateTime, turkeyTimeZone)))
                .ForMember(
                    dest => dest.DeletedAt,
                    opt => opt.MapFrom(src => TimeZoneInfo.ConvertTime(src.DeletedAt.UtcDateTime, turkeyTimeZone)))
                    .ForMember(
                        dest => dest.Categories,
                        opt => opt.MapFrom(src => src.ProductCategories.Select(pc => pc.Category)))
                        .ForMember(
                            dest => dest.IsInStock,
                            opt => opt.Ignore())
                            .ReverseMap();

        CreateMap<ProductCreateDto, Product>();
        CreateMap<ProductUpdateDto, Product>()
         .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())          // Manuel olarak UpdateAsync'te set ediliyor (baseUrl ile)
         .ForMember(dest => dest.ProductCategories, opt => opt.Ignore())  // Manuel olarak UpdateAsync'te yönetiliyor
         .ForMember(dest => dest.Slug, opt => opt.Ignore())               // Slug manuel olarak UpdateAsync'te güncelleniyor
         .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())          // CreatedAt değiştirilmemeli
         .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())          // Manuel olarak UpdateAsync'te set ediliyor
         .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())          // DeletedAt değiştirilmemeli
         .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())          // IsDeleted değiştirilmemeli
         .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<Product, ProductWithVariantsDto>()
            .IncludeBase<Product, ProductDto>()  // ✅ timezone conversion'ları da getirir
            .ForMember(dest => dest.StoneTypeName, opt => opt.MapFrom(src => src.StoneType != null ? src.StoneType.Name : null))
            .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.Color != null ? src.Color.Name : null))
            .ForMember(dest => dest.ColorHexCode, opt => opt.MapFrom(src => src.Color != null ? src.Color.HexCode : null))
  .ForMember(dest => dest.ProductVariants, opt => opt.MapFrom(src => src.ProductVariants.Where(v => !v.IsDeleted)))
.ForMember(dest => dest.ProductImages, opt => opt.MapFrom(src => src.ProductImages.Where(img => !img.IsDeleted)));
    }
}
