using System;
using NoraCollection.Shared.Dtos.CartDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface ICartService
{
    Task<ResponseDto<CartDto>> CreateCartAsync(CartCreateDto cartCreateDto);
    Task<ResponseDto<CartDto>> GetCartAsync(string userId);
    Task<ResponseDto<NoContentDto>> ClearCartAsync(string userId);
    Task<ResponseDto<CartItemDto>> AddToCartAsync(AddToCartDto addToCartDto);
    Task<ResponseDto<NoContentDto>> RemoveFromCartAsync(int cartItemId);
    Task<ResponseDto<NoContentDto>> ChangeQuantityAsync(ChangeQuantityDto changeQuantityDto);
    Task<ResponseDto<int>> GetCartItemCountAsync(string userId);
    Task<ResponseDto<decimal>> GetCartTotalAsync(string userId);
}
