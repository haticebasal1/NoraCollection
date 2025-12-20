using System;
using NoraCollection.Shared.Dtos.OrderDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IOrderService
{
 Task<ResponseDto<OrderDto>> CreateFormCartAsync(string userId, CheckoutDto checkoutDto);
 Task<ResponseDto<OrderDto>> GetOrderAsync(int id);
 Task<ResponseDto<OrderDto>> GetMyOrderAsync(int id, string userId);
 Task<ResponseDto<OrderDto>> GetByOrderNumberAsync(string orderNumber);
 Task<ResponseDto<IEnumerable<OrderDto>>> GetAllAsync(OrderFiltersDto orderFiltersDto);
 Task<ResponseDto<IEnumerable<OrderDto>>> GetMyOrdersAsync(string userId);
 Task<ResponseDto<NoContentDto>> ChangeOrderStatusAsync(ChangeOrderStatusDto changeOrderStatusDto);
 Task<ResponseDto<NoContentDto>> CancelOrderAsync(int id);
 Task<ResponseDto<int>> CountAsync(OrderFiltersDto? orderFiltersDto = null);
 Task<ResponseDto<int>> GetOrdersTotalAsync(OrderFiltersDto? orderFiltersDto = null);
}
