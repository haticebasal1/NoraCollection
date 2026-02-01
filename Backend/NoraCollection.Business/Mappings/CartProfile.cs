using System;
using System.Linq;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.CartDtos;

namespace NoraCollection.Business.Mappings;

public class CartProfile : Profile
{
    public CartProfile()
    {
        // Cart -> CartDto (liste / detay)
        CreateMap<Cart, CartDto>()
            .ForMember(dest => dest.CartItems, opt => opt.MapFrom(src => src.CartItems))
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.ItemsCount, opt => opt.Ignore())
            .ForMember(dest => dest.CouponId, opt => opt.MapFrom(src => src.CouponId))
            .ForMember(dest => dest.CouponCode, opt => opt.MapFrom(src => src.Coupon != null ? src.Coupon.Code : null))
            .ForMember(dest => dest.CouponDiscountAmount, opt => opt.MapFrom(src => src.Coupon != null ? src.Coupon.DiscountAmount : 0));

        // CartItem -> CartItemDto (ProductName, VariantName, ImageUrl entity'den doldurulur)
        CreateMap<CartItem, CartItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : "Ürün Belirtilmemiş"))
            .ForMember(dest => dest.VariantName, opt => opt.MapFrom(src => src.ProductVariant != null
                ? (src.ProductVariant.Color != null ? src.ProductVariant.Color.Name + " / " : "") + src.ProductVariant.Size
                : "Standart"))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product != null ? src.Product.ImageUrl : null))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.ItemTotal, opt => opt.Ignore());

        // CartCreateDto -> Cart (yeni sepet; hediye alanları service'te set edilir)
        CreateMap<CartCreateDto, Cart>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.GiftNote, opt => opt.Ignore())
            .ForMember(dest => dest.IsGiftPackage, opt => opt.Ignore())
            .ForMember(dest => dest.GiftOptionId, opt => opt.Ignore())
            .ForMember(dest => dest.GiftOption, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CartItems, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // CartItemCreateDto -> CartItem (CartId ve UnitPrice service'te set edilir)
        CreateMap<CartItemCreateDto, CartItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CartId, opt => opt.Ignore())
            .ForMember(dest => dest.Cart, opt => opt.Ignore())
            .ForMember(dest => dest.UnitPrice, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.ProductVariant, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}
