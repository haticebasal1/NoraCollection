using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class ProductVariant : BaseEntity, IEntity
{
    public ProductVariant(int productId, string size, int stock)
    {
        ProductId = productId;
        Size = size;
        Stock = stock;
    }

    private ProductVariant() { }

    public int ProductId { get; set; }                  // Hangi ürüne ait (Foreign Key)
    public Product Product { get; set; } = null!;       // Navigation property

    public string Size { get; set; } = null!;           // Ölçü: "S", "M", "L" veya "16cm", "18cm", "20cm"
    public string? SKU { get; set; }                    // Stok kodu: "YZK-ALTIN-16" (depo takibi için)
    public int Stock { get; set; } = 0;                 // Bu varyantın stoğu
    public decimal? PriceAdjustment { get; set; }       // Fiyat farkı: Büyük beden +50₺ gibi

    public bool IsInStock => Stock > 0;                 // Hesaplanan: Stokta var mı?
}
