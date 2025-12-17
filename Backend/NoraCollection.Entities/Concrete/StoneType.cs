using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class StoneType : BaseEntity, IEntity
{
    public StoneType(string name, string slug)
    {
        Name = name;
        Slug = slug;
    }

    private StoneType() { }

    public string Name { get; set; } = null!;           // Taş adı: "Akik", "Ametist", "İnci", "Turkuaz"
    public string Slug { get; set; } = null!;           // URL için: "akik", "ametist" (SEO dostu)
    public string? Description { get; set; }            // Taş hakkında bilgi
    public string? ImageUrl { get; set; }               // Taş görseli

    public ICollection<Product> Products { get; set; } = [];  // Bu taş tipine sahip ürünler
}
