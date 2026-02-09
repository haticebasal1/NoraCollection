using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.OrderCouponDtos;
using NoraCollection.Shared.Dtos.OrderDtos;

namespace NoraCollection.Business.Mappings;

public class OrderProfile:Profile
{
   public OrderProfile()
    {
        var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
        CreateMap<Order,OrderDto>()
        .ForMember(
            dest=> dest.OrderDate,
            opt=> opt.MapFrom(src=> TimeZoneInfo.ConvertTime(src.CreatedAt.UtcDateTime,turkeyTimeZone)))
            .ForMember(
              dest=>dest.CanceledDate,
              opt=>opt.MapFrom(src=>TimeZoneInfo.ConvertTime(src.DeletedAt.UtcDateTime,turkeyTimeZone)))
              .ForMember(
                dest=>dest.OrderStatusUpdatedDate,
                opt=>opt.MapFrom(src=>TimeZoneInfo.ConvertTime(src.UpdatedAt.UtcDateTime,turkeyTimeZone)))
                .ForMember(
                 dest=>dest.OrderItems,
                 opt=>opt.MapFrom(src => src.OrderItems))
                 .ForMember(
                    dest=>dest.OrderCoupons,
                    opt=>opt.MapFrom(src=>src.OrderCoupons))
                    .ForMember(
                        dest=>dest.User,
                        opt=>opt.Ignore())
                    .ReverseMap();
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(
                    dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(
                    dest => dest.ProductImageUrl,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.ImageUrl : null))
                .ForMember(
                    dest => dest.VariantName,
                    opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.Size : null))
                .ReverseMap();
                    CreateMap<OrderItemCreateDto,OrderItem>();
            CreateMap<OrderCoupon, OrderCouponDto>()
                .ForMember(
                    dest => dest.CouponCode,
                    opt => opt.MapFrom(src => src.Coupon != null ? src.Coupon.Code : null))
                .ForMember(
                    dest => dest.DiscountAmount,
                    opt => opt.MapFrom(src => src.Coupon != null ? src.Coupon.DiscountAmount : 0m))
                .ReverseMap();
    }
}
