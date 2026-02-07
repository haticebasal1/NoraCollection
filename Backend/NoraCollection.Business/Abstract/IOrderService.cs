using System;
using NoraCollection.Shared.Dtos.OrderDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IOrderService
{
  Task<ResponseDto<OrderDto>> CreateOrderAsync(CheckoutDto checkoutDto);
  Task<ResponseDto<OrderDto>> OrderNowAsync(OrderNowDto orderNowDto);
  Task<ResponseDto<IEnumerable<OrderDto>>> GetMyOrdersAsync(string userId);
  Task<ResponseDto<OrderDto>> GetOrderAsync(int id);
  Task<ResponseDto<OrderDto>> GetMyOrderByIdAsync(int id,string userId);
  Task<ResponseDto<NoContentDto>> ChangeOrderStatusAsync(ChangeOrderStatusDto changeOrderStatusDto);
  Task<ResponseDto<IEnumerable<OrderDto>>> GetAllAsync(OrderFiltersDto orderFiltersDto);
  Task<ResponseDto<decimal>> GetOrdersTotalAsync(OrderFiltersDto orderFiltersDto);
  Task<ResponseDto<NoContentDto>> CancelOrderAsync(int id,string userId);
}
