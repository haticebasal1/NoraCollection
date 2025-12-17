using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class StaticPage : BaseEntity, IEntity
{
    public StaticPage(string title, string slug, string content)
    {
        Title = title;
        Slug = slug;
        Content = content;
    }

    private StaticPage()
    {

    }
    public string Title { get; set; } = null!;// Sayfa başlığı: "KVKK Aydınlatma Metni"
    public string Slug { get; set; } = null!;// URL için: "kvkk", "mesafeli-satis"
    public string Content { get; set; } = null!; // Sayfa içeriği (HTML)
    public string? MetaTitle { get; set; }              // SEO başlık
    public string? MetaDescription { get; set; }        // SEO açıklama
    public bool IsPublished { get; set; } = true;      // Yayında mı?
    public int DisplayOrder { get; set; } = 0;// Sıralama
}
