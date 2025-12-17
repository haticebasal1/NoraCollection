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
    public string HexCode { get; set; } = null!;        // Renk kodu: "#D4B773", "#C0C0C0" (frontend renk gösterimi)
    public string? Slug { get; set; }                   // URL için: "altin", "gumus"

    public ICollection<Product> Products { get; set; } = [];  // Bu renkteki ürünler
}
