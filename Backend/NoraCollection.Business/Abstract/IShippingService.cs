using System;
using NoraCollection.Shared.Dtos.ResponseDtos;
using NoraCollection.Shared.Dtos.ShippingDtos;

namespace NoraCollection.Business.Abstract;

public interface IShippingService
{
Task<ResponseDto<ShippingDto>> GetByIdAsync(int id);
Task<ResponseDto<ShippingDto>> GetByOrderIdAsync(int orderId);
Task<ResponseDto<ShippingDto>> GetByTrackingNumberAsync(string trackingNumber);
Task<ResponseDto<IEnumerable<ShippingDto>>> GetAllAsync();
Task<ResponseDto<ShippingDto>> AddAsync(ShippingCreateDto shippingCreateDto);// Kargo bilgisi ekle
Task<ResponseDto<NoContentDto>> UpdateAsync(ShippingUpdateDto shippingUpdateDto);
Task<ResponseDto<NoContentDto>> DeleteAsync(int id);
Task<ResponseDto<NoContentDto>> MarkAsDeliveredAsync(int id);// Teslim edildi
}
