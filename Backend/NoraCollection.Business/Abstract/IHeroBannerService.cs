using System;
using NoraCollection.Shared.Dtos.HeroBannerDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IHeroBannerService
{
  Task<ResponseDto<HeroBannerDto>> GetByIdAsync(int id);
  Task<ResponseDto<IEnumerable<HeroBannerDto>>> GetAllAsync(bool? isActive = null);
  Task<ResponseDto<IEnumerable<HeroBannerDto>>> GetActiveAsync();
  Task<ResponseDto<HeroBannerDto>> AddAsync(HeroBannerCreateDto heroBannerCreateDto);
  Task<ResponseDto<NoContentDto>> UpdateAsync(HeroBannerUpdateDto heroBannerUpdateDto);
  Task<ResponseDto<NoContentDto>> DeleteAsync(int id);
  Task<ResponseDto<NoContentDto>> ActivateAsync(int id);
  Task<ResponseDto<NoContentDto>> DeactivateAsync(int id);
  Task<ResponseDto<NoContentDto>> UpdateDisplayOrderAsync(int id,int newOrder);
}
