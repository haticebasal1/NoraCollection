using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.CouponDtos;

namespace NoraCollection.Business.Mappings;

public class CouponProfile : Profile
{
    public CouponProfile()
    {
        var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
        CreateMap<Coupon,CouponDto>()
        .ForMember(
            dest=>dest.ExpirationDate,
            opt=>opt.MapFrom(src=>TimeZoneInfo.ConvertTime(src.ExpiryDate,turkeyTimeZone)))
            .ReverseMap();
        CreateMap<CouponCreateDto,Coupon>()
        .ForMember(
            dest=>dest.UsedCount,
            opt=>opt.Ignore());
        CreateMap<CouponUpdateDto,Coupon>()
        .ForMember(
            dest=>dest.UsedCount,
            opt=>opt.Ignore());    
    }
}
