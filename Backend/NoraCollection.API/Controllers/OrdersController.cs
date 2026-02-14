using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoraCollection.API.Controllers.BaseController;
using NoraCollection.Business.Abstract;
using NoraCollection.Shared.Dtos.OrderDtos;
using NoraCollection.Shared.Enums;

namespace NoraCollection.API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : CustomControllerBase
    {
        private readonly IOrderService _orderManager;

        public OrdersController(IOrderService orderManager)
        {
            _orderManager = orderManager;
        }
        [HttpPost("checkout")]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] CheckoutDto checkoutDto)
        {
            checkoutDto.UserId= UserId;
            var response = await _orderManager.CreateOrderAsync(checkoutDto);
            return CreateResult(response);
        }
        [HttpPost("ordernow")]
        [Authorize]
        public async Task<IActionResult> OrderNow([FromBody] OrderNowDto orderNowDto)
        {
            orderNowDto.UserId = UserId;
            var response = await _orderManager.OrderNowAsync(orderNowDto);
            return CreateResult(response);
        }
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var response = await _orderManager.GetOrderAsync(id);
            return CreateResult(response);
        }
        [HttpGet("myorder/{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetMyOrder(int id)
        {
            var response = await _orderManager.GetMyOrderByIdAsync(id,UserId!);
            return CreateResult(response);
        }
        [HttpPut("{orderId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeOrderStatus(int orderId,[FromQuery] OrderStatus orderStatus)
        {
            var response = await _orderManager.ChangeOrderStatusAsync(new ChangeOrderStatusDto
            {
                OrderId = orderId,
                OrderStatus=orderStatus
            });
            return CreateResult(response);
        }
        [HttpPut("cancel/{id:int}")]
        [Authorize]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var response = await _orderManager.CancelOrderAsync(id,UserId!);
            return CreateResult(response);
        }
       [HttpGet("all")]
       [Authorize(Roles ="Admin")]
       public async Task<IActionResult> GetAllOrders([FromQuery] OrderFiltersDto orderFilters)
        {
            var response = await _orderManager.GetAllAsync(orderFilters);
            return CreateResult(response);
        } 
        [HttpGet("myorders")]
        [Authorize]
        public async Task<IActionResult> GetMyOrders([FromQuery] int pageNumber=1,[FromQuery] int pageSize = 20)
        {
            var response = await _orderManager.GetMyOrdersAsync(UserId!,pageNumber,pageSize);
            return CreateResult(response);
        }
        [HttpGet("total")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrdersTotal([FromQuery] OrderFiltersDto orderFilter)
        {
            var response = await _orderManager.GetOrdersTotalAsync(orderFilter);
            return CreateResult(response);
        }
    }
}
