using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.CartDtos;

namespace NoraCollection.Business.Mappings;

public class CartProfile:Profile
{
  public CartProfile()
    {
        CreateMap<Cart,CartDto>()
        .ForMember(
            dest=>dest.User,
            opt=>opt.MapFrom(src=>src.User))
            .ForMember(
                dest=>dest.CartItems,
                opt=>opt.MapFrom(src=>src.CartItems))
                .ReverseMap();

        CreateMap<CartItem,CartItemDto>()
        .ForMember(
            dest=>dest.Product,
            opt=>opt.MapFrom(src=>src.Product))
            .ReverseMap();
     CreateMap<CartCreateDto,Cart>();
     CreateMap<CartItemCreateDto,CartItem>();
    }
}
