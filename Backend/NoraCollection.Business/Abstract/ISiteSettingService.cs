using System;
using NoraCollection.Shared.Dtos.ResponseDtos;
using NoraCollection.Shared.Dtos.SiteSettingDtos;

namespace NoraCollection.Business.Abstract;

public interface ISiteSettingService
{
    Task<ResponseDto<SiteSettingDto>> GetByIdAsync(int id);
    Task<ResponseDto<SiteSettingDto>> GetByKeyAsync(string key);
    Task<ResponseDto<string>> GetValueAsync(string key);
    Task<ResponseDto<IEnumerable<SiteSettingDto>>> GetAllAsync(string? group = null);
    Task<ResponseDto<IEnumerable<SiteSettingDto>>> GetByGroupAsync(string group);
    Task<ResponseDto<SiteSettingDto>> AddAsync(SiteSettingCreateDto siteSettingCreateDto);
    Task<ResponseDto<NoContentDto>> UpdateAsync(SiteSettingUpdateDto siteSettingUpdateDto);
    Task<ResponseDto<NoContentDto>> UpdateValueAsync(string key, string value);
    Task<ResponseDto<NoContentDto>> DeleteAsync(int id);
}
