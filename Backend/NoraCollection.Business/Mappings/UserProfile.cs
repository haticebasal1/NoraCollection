using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.AuthDtos;

namespace NoraCollection.Business.Mappings;

public class UserProfile:Profile
{
  public UserProfile()
    {
        CreateMap<User,UserDto>().ReverseMap();
    }
}
