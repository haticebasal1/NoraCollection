using System;
using NoraCollection.Shared.Dtos.ProductDtos;

namespace NoraCollection.Shared.Dtos.CartDtos;

public class CartItemDto
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public string? ImageUrl { get; set; }
    public ProductDto? Product { get; set; }
    public int Quantity { get; set; }
    public decimal ItemTotal => (Product?.Price ?? 0) * Quantity;
}
