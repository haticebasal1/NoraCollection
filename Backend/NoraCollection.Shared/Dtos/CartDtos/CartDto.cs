using System;
using NoraCollection.Shared.Dtos.AuthDtos;

namespace NoraCollection.Shared.Dtos.CartDtos;

public class CartDto
{
public int Id { get; set; }
public string? UserId { get; set; }
public UserDto? User { get; set; }
public ICollection<CartItemDto> CartItems { get; set; }= new List<CartItemDto>();
public decimal TotalAmount => CartItems.Sum(x=>x.ItemTotal);
public int ItemsCount => CartItems.Sum(x => x.Quantity);
public string? ImageUrl { get; set; }
}
