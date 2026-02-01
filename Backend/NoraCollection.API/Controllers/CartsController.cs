using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoraCollection.API.Controllers.BaseController;
using NoraCollection.Business.Abstract;
using NoraCollection.Shared.Dtos.CartDtos;

namespace NoraCollection.API.Controllers
{
    [Route("api/carts")]
    [ApiController]
    [Authorize]
    public class CartsController : CustomControllerBase
    {
        private readonly ICartService _cartManager;

        public CartsController(ICartService cartManager)
        {
            _cartManager = cartManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var response = await _cartManager.GetCartAsync(UserId!);
            return CreateResult(response);
        }
        [HttpGet("count")]
        public async Task<IActionResult> GetCartItemsCount()
        {
            var response = await _cartManager.GetCartItemsCountAsync(UserId!);
            return CreateResult(response);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto addToCartDto)
        {
            addToCartDto.UserId = UserId;
            var response = await _cartManager.AddToCartAsync(addToCartDto);
            return CreateResult(response);
        }
        [HttpPut]
        public async Task<IActionResult> ChangeQuantity([FromBody] ChangeQuantityDto changeQuantityDto)
        {
            changeQuantityDto.UserId = UserId;
            var response = await _cartManager.ChangeQuantityAsync(changeQuantityDto);
            return CreateResult(response);
        }
        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var response = await _cartManager.RemoveFromCartAsync(cartItemId, UserId!);
            return CreateResult(response);
        }
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var response = await _cartManager.ClearCartAsync(UserId!);
            return CreateResult(response);
        }
        [HttpPut("options")]
        public async Task<IActionResult> UpdateCartOptions([FromBody] UpdateCartOptionsDto updateCartOptionsDto)
        {
            var response = await _cartManager.UpdateCartOptionAsync(UserId!, updateCartOptionsDto);
            return CreateResult(response);
        }
        [HttpPost("coupon")]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponRequest request)
        {
            var response = await _cartManager.ApplyCouponAsync(UserId!, request.CouponCode ?? "");
            return CreateResult(response);
        }
        [HttpDelete("coupon")]
        public async Task<IActionResult> RemoveCoupon()
        {
            var response = await _cartManager.RemoveCouponAsync(UserId!);
            return CreateResult(response);
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateCart()
        {
            var createCartDto = new CartCreateDto { UserId = UserId };
            var response = await _cartManager.CreateCartAsync(createCartDto);
            return CreateResult(response);
        }
        public class ApplyCouponRequest
        {
            public string? CouponCode { get; set; }
        }
    }
}
