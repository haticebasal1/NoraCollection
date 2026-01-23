using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.StoneTypeDtos;

namespace NoraCollection.Business.Mappings;

public class StoneTypeProfile : Profile
{
    public StoneTypeProfile()
    {
        CreateMap<StoneType, StoneTypeDto>();
        CreateMap<StoneTypeCreateDto, StoneType>()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
        .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
        .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
        .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        CreateMap<StoneTypeUpdateDto, StoneType>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
        .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
        .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
        .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
        .ForMember(dest=>dest.IsActive,opt=>opt.Ignore())
        .ForMember(dest=>dest.DisplayOrder, opt=>opt.Ignore());
    }
}
