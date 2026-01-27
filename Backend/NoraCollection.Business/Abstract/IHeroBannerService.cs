using System;
using NoraCollection.Shared.Dtos.HeroBannerDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IHeroBannerService
{
    // Admin Paneli İçin: Tüm bannerları (aktif/pasif/silinmiş) getirir
    Task<ResponseDto<IEnumerable<HeroBannerDto>>> GetAllAsync(bool? isdeleted = false, bool? isActive = null);
    // ✅ UI/Frontend İçin: Sadece şu an yayında olması gerekenleri getirir
    // (Aktif olan ve tarihi bugünle uyumlu olanlar)
    Task<ResponseDto<IEnumerable<HeroBannerDto>>> GetActiveBannersAsync();
    Task<ResponseDto<HeroBannerDto>> GetByIdAsync(int id);
    Task<ResponseDto<HeroBannerDto>> AddAsync(HeroBannerCreateDto heroBannerCreateDto);
    Task<ResponseDto<NoContentDto>> UpdateAsync(HeroBannerUpdateDto heroBannerUpdateDto);
    Task<ResponseDto<NoContentDto>> SoftDeleteAsync(int id);
    Task<ResponseDto<NoContentDto>> HardDeleteAsync(int id);
    // Sıralama değiştirmek için hızlı bir metod
    Task<ResponseDto<NoContentDto>> UpdateOrderAsync(int id, int newOrder);
}
