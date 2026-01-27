using System;
using AutoMapper;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.HeroBannerDtos;

namespace NoraCollection.Business.Mappings;

public class HeroBannerProfile : Profile
{
    public HeroBannerProfile()
    {
        // Entity -> Dto (Listeleme için)
        CreateMap<HeroBanner, HeroBannerDto>();

        // CreateDto -> Entity
        CreateMap<HeroBannerCreateDto, HeroBanner>()
            // Resimler Manager içinde manuel set edildiği için Ignore edilmesi şart, doğru yapmışsın.
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.MobileImageUrl, opt => opt.Ignore())
            // Ortak alanları topluca ignore etmek yerine bazen sadece
            // değişmemesi gerekenleri yazmak yeterlidir.
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // UpdateDto -> Entity
        CreateMap<HeroBannerUpdateDto, HeroBanner>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.MobileImageUrl, opt => opt.Ignore());
    }
}
