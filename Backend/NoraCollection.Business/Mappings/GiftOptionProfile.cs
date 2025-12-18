using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.GiftOptionDtos;

namespace NoraCollection.Business.Mappings;

public class GiftOptionProfile:Profile
{
 public GiftOptionProfile()
    {
        CreateMap<GiftOption,GiftOptionDto>().ReverseMap();
        CreateMap<GiftOptionCreateDto,GiftOption>();
        CreateMap<GiftOptionUpdateDto,GiftOption>();
    }
}
