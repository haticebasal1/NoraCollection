using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class CartItem : BaseEntity, IEntity
{
    private CartItem() { }
    
    public CartItem(int cartId, int productId, int? productVariantId, int quantity, decimal unitPrice)
    {
        CartId = cartId;
        ProductId = productId;
        ProductVariantId = productVariantId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public int CartId { get; set; }
    public Cart? Cart { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    // KRİTİK: Varyant desteği (Yüzük ölçüsü, taş tipi vb. için)
    public int? ProductVariantId { get; set; }
    public ProductVariant? ProductVariant { get; set; }

    public int Quantity { get; set; }

    // KRİTİK: Sepete eklendiği andaki birim fiyat
    public decimal UnitPrice { get; set; }

    // Hesaplanan alan (Read-only veya DB computed olabilir)
    public decimal ItemTotal => UnitPrice * Quantity;
}
