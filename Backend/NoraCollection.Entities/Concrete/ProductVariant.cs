using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class ProductVariant : BaseEntity, IEntity
{
    public ProductVariant(int productId, string size)
    {
        ProductId = productId;
        Size = size;
    }

    private ProductVariant() { }

    public string Size { get; set; } = null!;           // Beden: "14", "15", "16", "17", "S", "M", "L"
    public string? Sku { get; set; }                    // Stok kodu: "YZK-AKK-14-001"
    public int Stock { get; set; } = 0;                 // Bu varyantın stoğu
    public decimal? PriceAdjustment { get; set; }       // Fiyat farkı (büyük beden için +50 TL gibi)
    public bool IsAvailable { get; set; } = true;       // Satışta mı?

    // İlişkiler
    public int ProductId { get; set; }                  // Hangi ürüne ait (Foreign Key)
    public Product Product { get; set; } = null!;       // Navigation property
}




