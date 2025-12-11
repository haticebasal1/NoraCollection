using System;

namespace NoraCollection.Shared.Dtos.CartDtos;

public class CartCreateDto
{
    public string? UserId { get; set; }  // Sepet kime ait?
    public List<CartItemCreateDto>? CartItems { get; set; } // İster boş gelir, ister ürünle gelir
}
