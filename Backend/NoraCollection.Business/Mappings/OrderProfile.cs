using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
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
                 .ReverseMap();

    }
}
