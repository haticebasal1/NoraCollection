using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class Color : BaseEntity, IEntity
{
    public Color(string name, string hexCode)
    {
        Name = name;
        HexCode = hexCode;
    }

    private Color() { }

    public string Name { get; set; } = null!;           // Renk adı: "Altın", "Gümüş", "Rose Gold"
    public string HexCode { get; set; } = null!;        // Renk kodu: "#FFD700", "#C0C0C0"
    public bool IsActive { get; set; } = true;          // Aktif mi? (Filtrede gösterilsin mi?)
    public int DisplayOrder { get; set; } = 0;          // Sıralama

    // Navigation
    public ICollection<Product> Products { get; set; } = [];
}















