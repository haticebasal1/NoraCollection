using System;
using NoraCollection.Shared.Dtos.GiftOptionDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IGiftOptionService
{
    Task<ResponseDto<GiftOptionDto>> GetByIdAsync(int id);
    Task<ResponseDto<IEnumerable<GiftOptionDto>>> GetAllAsync(bool? isActive = null);
    Task<ResponseDto<IEnumerable<GiftOptionDto>>> GetActiveAsync();// Checkout sayfası için
    Task<ResponseDto<GiftOptionDto>> AddAsync(GiftOptionCreateDto giftOptionCreateDto);
    Task<ResponseDto<NoContentDto>> UpdateAsync(GiftOptionUpdateDto giftOptionUpdateDto);
    Task<ResponseDto<NoContentDto>> DeleteAsync(int id);
    Task<ResponseDto<NoContentDto>> ActivateAsync(int id);
    Task<ResponseDto<NoContentDto>> DeactivateAsync(int id);
}
