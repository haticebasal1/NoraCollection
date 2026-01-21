using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class ProductImage : BaseEntity, IEntity
{
    public ProductImage(int productId, string imageUrl)
    {
        ProductId = productId;
        ImageUrl = imageUrl;
    }

    private ProductImage() { }

    public string ImageUrl { get; set; } = null!;       // Resim URL'i
    public string? AltText { get; set; }                // SEO için alternatif metin
    public int DisplayOrder { get; set; } = 0;          // Sıralama (ilk resim ana görsel)
    public bool IsMain { get; set; } = false;           // Ana görsel mi?

    // İlişkiler
    public int ProductId { get; set; }                  // Hangi ürüne ait (Foreign Key)
    public Product Product { get; set; } = null!;       // Navigation property
}










