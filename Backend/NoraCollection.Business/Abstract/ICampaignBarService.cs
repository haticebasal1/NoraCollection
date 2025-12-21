using System;
using NoraCollection.Shared.Dtos.CampaignBarDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface ICampaignBarService
{
  Task<ResponseDto<CampaignBarDto>> GetByIdAsync(int id);
  Task<ResponseDto<IEnumerable<CampaignBarDto>>> GetAllAsync(bool? isActive = null);
  Task<ResponseDto<IEnumerable<CampaignBarDto>>> GetActiveAsync();//frontend için
  Task<ResponseDto<CampaignBarDto>> AddAsync(CampaignBarCreateDto campaignBarCreateDto);
  Task<ResponseDto<NoContentDto>> UpdateAsync(CampaignBarUpdateDto campaignBarUpdateDto);
  Task<ResponseDto<NoContentDto>> DeleteAsync(int id);
  Task<ResponseDto<NoContentDto>> ActivateAsync(int id);
  Task<ResponseDto<NoContentDto>> DeactivateAsync(int id);
}
