using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class StoneType : BaseEntity, IEntity
{
    public StoneType(string name)
    {
        Name = name;
    }

    private StoneType() { }

    public string Name { get; set; } = null!;           // Taş adı: "Akik", "Ametist", "Turkuaz"
    public string? Description { get; set; }            // Taş hakkında bilgi
    public string? ImageUrl { get; set; }               // Taş görseli
    public bool IsActive { get; set; } = true;          // Aktif mi? (Filtrede gösterilsin mi?)
    public int DisplayOrder { get; set; } = 0;          // Sıralama

    // Navigation
    public ICollection<Product> Products { get; set; } = [];
}











