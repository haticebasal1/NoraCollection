using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class BlogPost : BaseEntity, IEntity
{
    public BlogPost(string title, string slug, string content, int blogCategoryId)
    {
        Title = title;
        Slug = slug;
        Content = content;
        BlogCategoryId = blogCategoryId;
    }

    private BlogPost() { }

    public string Title { get; set; } = null!;          // Yazı başlığı: "Akik Taşının Faydaları"
    public string Slug { get; set; } = null!;           // URL için: "akik-tasinin-faydalari"
    public string? Summary { get; set; }                // Kısa özet: Liste görünümünde gösterilir
    public string Content { get; set; } = null!;        // Ana içerik: HTML formatında yazı
    public string? FeaturedImageUrl { get; set; }       // Kapak görseli

    // SEO Alanları
    public string? MetaTitle { get; set; }              // SEO başlık: Google'da görünen başlık
    public string? MetaDescription { get; set; }        // SEO açıklama: Google'da görünen açıklama
    public string? MetaKeywords { get; set; }           // Anahtar kelimeler

    // Durum
    public bool IsPublished { get; set; } = false;      // Yayında mı? false ise taslak
    public DateTime? PublishedAt { get; set; }          // Yayın tarihi
    public int ViewCount { get; set; } = 0;             // Görüntülenme sayısı

    // İlişkiler
    public int BlogCategoryId { get; set; }             // Hangi kategoride (Foreign Key)
    public BlogCategory BlogCategory { get; set; } = null!;  // Navigation property

    public string? AuthorId { get; set; }               // Yazar Id (opsiyonel)
    public User? Author { get; set; }                   // Yazar navigation property
}
