using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class BlogCategory : BaseEntity, IEntity
{
    public BlogCategory(string name, string slug)
    {
        Name = name;
        Slug = slug;
    }

    private BlogCategory() { }

    public string Name { get; set; } = null!;           // Kategori adı: "Doğal Taş Bilgileri", "Takı Rehberi"
    public string Slug { get; set; } = null!;           // URL için: "dogal-tas-bilgileri"
    public string? Description { get; set; }            // Kategori açıklaması

    public ICollection<BlogPost> BlogPosts { get; set; } = [];  // Bu kategorideki yazılar
}
