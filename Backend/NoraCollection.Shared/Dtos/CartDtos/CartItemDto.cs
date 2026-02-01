using System;
using NoraCollection.Shared.Dtos.ProductDtos;

namespace NoraCollection.Shared.Dtos.CartDtos;

public class CartItemDto
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; } // Hızlı erişim için
    
    // Kritik: Senin senaryonda varyantlar (renk, ölçü vb.) var
    public int? ProductVariantId { get; set; } 
    public string? VariantName { get; set; } // Örn: "Mavi / 14 Numara"
    
    public string? ImageUrl { get; set; } // Ürünün veya varyantın resmi
    public int Quantity { get; set; }
    
    // Fiyatı Product yerine direkt burada tutmak veya Variant üzerinden almak daha güvenli
    public decimal UnitPrice { get; set; } 
    public decimal ItemTotal => UnitPrice * Quantity;
}
