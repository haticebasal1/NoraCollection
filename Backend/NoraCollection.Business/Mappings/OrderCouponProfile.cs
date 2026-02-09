using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.OrderCouponDtos;

namespace NoraCollection.Business.Mappings;

public class OrderCouponProfile : Profile
{
    public OrderCouponProfile()
    {
        CreateMap<OrderCoupon, OrderCouponDto>()
            .ForMember(
                dest => dest.CouponCode,
                opt => opt.MapFrom(src => src.Coupon != null ? src.Coupon.Code : null))
            .ForMember(
                dest => dest.DiscountAmount,
                opt => opt.MapFrom(src => src.Coupon != null ? src.Coupon.DiscountAmount : 0m))
            .ReverseMap()
        .ForMember(
            dest => dest.Coupon,
            opt => opt.Ignore())
            .ForMember(
                dest => dest.Order,
                opt => opt.Ignore());

        CreateMap<OrderCouponCreateDto, OrderCoupon>()
        .ForMember(
            dest => dest.OrderId,
            opt => opt.Ignore())
        .ForMember(
            dest => dest.Coupon,
            opt => opt.Ignore())
        .ForMember(
            dest => dest.Order,
            opt => opt.Ignore());
    }
}
