using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.ShippingDtos;

namespace NoraCollection.Business.Mappings;

public class ShippingProfile:Profile
{
 public ShippingProfile()
    {
        var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
        CreateMap<Shipping,ShippingDto>()
        .ForMember(
            dest=>dest.ShippedDate,
            opt=>opt.MapFrom(src=>TimeZoneInfo.ConvertTime(src.ShippedDate,turkeyTimeZone)))
            .ForMember(
                dest=>dest.DeliveredDate,
                opt=>opt.MapFrom(src=>src.DeliveredDate.HasValue
                ?(DateTime?)TimeZoneInfo.ConvertTime(src.DeliveredDate.Value, turkeyTimeZone):null))
                .ReverseMap()
            .ForMember(
                dest=>dest.Order,
                opt=>opt.Ignore());

        CreateMap<ShippingCreateDto,Shipping>()
        .ForMember(
            dest=>dest.Order,
            opt=>opt.Ignore());
        CreateMap<ShippingUpdateDto,Shipping>()
        .ForMember(
            dest=>dest.Order,
            opt=>opt.Ignore());
    }
}
