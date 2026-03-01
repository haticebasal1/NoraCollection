using System;
using NoraCollection.Shared.Dtos.CampaignBarDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface ICampaignBarService
{
    // Ana sayfa üst bar için: Aktif, silinmemiş ve tarih aralığında olan kampanya barlarını getirir.
    Task<ResponseDto<IEnumerable<CampaignBarDto>>> GetActiveBarsAsync();
    // Admin için: Tüm kampanya barlarını getirir (opsiyonel filtrelerle).
    Task<ResponseDto<IEnumerable<CampaignBarDto>>> GetAllAsync(bool? includeDeleted = false, bool? isActive = null);
    // Id ile tek kampanya bar getirir.
    Task<ResponseDto<CampaignBarDto>> GetByIdAsync(int id);
    // Yeni kampanya bar ekler.
    Task<ResponseDto<CampaignBarDto>> AddAsync(CampaignBarCreateDto campaignBarCretateDto);
    //Güncelleme
    Task<ResponseDto<NoContentDto>> UpdateAsync(CampaignBarUpdateDto campaignBarUpdateDto);
    Task<ResponseDto<NoContentDto>> SoftDeleteAsync(int id);
}
