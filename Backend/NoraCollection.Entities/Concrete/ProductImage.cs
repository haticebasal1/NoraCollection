using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class ProductImage : BaseEntity, IEntity
{
    public ProductImage(int productId, string imageUrl, int displayOrder)
    {
        ProductId = productId;
        ImageUrl = imageUrl;
        DisplayOrder = displayOrder;
    }

    private ProductImage() { }

    public int ProductId { get; set; }                  // Hangi ürüne ait (Foreign Key)
    public Product Product { get; set; } = null!;       // Navigation property

    public string ImageUrl { get; set; } = null!;       // Görsel yolu: "products/yuzuk-1.png"
    public string? AltText { get; set; }                // SEO için alt yazı: "Altın yüzük yan görünüm"
    public int DisplayOrder { get; set; } = 0;          // Sıralama: slider'da hangi sırada gösterilecek
    public bool IsMain { get; set; } = false;           // Ana görsel mi? Liste görünümünde bu gösterilir
}
