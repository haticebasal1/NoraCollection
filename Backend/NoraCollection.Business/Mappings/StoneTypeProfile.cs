using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.StoneTypeDtos;

namespace NoraCollection.Business.Mappings;

public class StoneTypeProfile:Profile
{
 public StoneTypeProfile()
    {
        CreateMap<StoneType,StoneTypeDto>().ReverseMap();
        CreateMap<StoneTypeCreateDto,StoneType>();
        CreateMap<StoneTypeUpdateDto,StoneType>();
    }
}
