using System;
using NoraCollection.Shared.Dtos.CouponDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface ICouponService
{
    Task<ResponseDto<CouponDto>> GetByIdAsync(int id);
    Task<ResponseDto<CouponDto>> GetByCodeAsync(string code);
    Task<ResponseDto<IEnumerable<CouponDto>>> GetAllAsync(bool? isActive = null);
    Task<ResponseDto<CouponDto>> AddAsync(CouponCreateDto couponCreateDto);
    Task<ResponseDto<NoContentDto>> UpdateAsync(CouponUpdateDto couponUpdateDto);
    Task<ResponseDto<NoContentDto>> DeleteAsync(int id);
    Task<ResponseDto<CouponDto>> ValidateAsync(string code); // Kupon geçerli mi?
    Task<ResponseDto<decimal>> CalculateDiscountAsync(string code, decimal cartTotal); // İndirim tutarı
    Task<ResponseDto<NoContentDto>> IncrementUsageAsync(string code);// Kullanım sayısını artır
    Task<ResponseDto<NoContentDto>> ActivateAsync(int id);
    Task<ResponseDto<NoContentDto>> DeactivateAsync(int id);
}
