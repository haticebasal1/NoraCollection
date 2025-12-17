using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class Product : BaseEntity, IEntity
{
    public Product(string? name, string? properties, decimal price, string? imageUrl, bool isHome)
    {
        Name = name;
        Properties = properties;
        Price = price;
        ImageUrl = imageUrl;
        IsHome = isHome;
    }

    private Product() { }
    public string? Name { get; set; }
    public string? Properties { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsHome { get; set; }
    public int Stock { get; set; } = 0;          // veritabanında saklanır
    public bool IsInStock => Stock > 0;          // sadece okunur, kolay kullanım
    public string? Slug { get; set; }         // "altin-yuzuk" - URL için
    public decimal? DiscountedPrice { get; set; } // İndirimli fiyat

    // Açıklamalar (Ürün Detay Sayfası için)
    public string? Description { get; set; }     // Uzun açıklama
    public string? StoneInfo { get; set; }      // Doğal taş bilgisi
    public string? UsageSuggestion { get; set; } // Kullanım önerisi
    public string? DesignStory { get; set; }     // Tasarım hikâyesi

    // Etiketler (Ana sayfa bölümleri için)
    public bool IsBestSeller { get; set; }
    public bool IsNewArrival { get; set; }
    public bool IsFeatured { get; set; }

    // Filtre İlişkileri
    public int? StoneTypeId { get; set; }
    public StoneType? StoneType { get; set; }

    public int? ColorId { get; set; }
    public Color? Color { get; set; }

    public ICollection<ProductCategory> ProductCategories { get; set; } = [];
    public ICollection<ProductImage> ProductImages { get; set; } = [];
    public ICollection<ProductVariant> ProductVariants { get; set; } = [];
}
