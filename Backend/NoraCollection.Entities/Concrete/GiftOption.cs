using System;

namespace NoraCollection.Entities.Concrete;

public class GiftOption
{
    public GiftOption(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    private GiftOption()
    {
        
    }
    public string Name { get; set; } = null!; // Ad: "Hediye Paketi", "Özel Kutu"
    public string? Description { get; set; }// Açıklama
    public string? ImageUrl { get; set; } // Görsel
    public decimal Price { get; set; }// Fiyat: 0 = ücretsiz
    public bool IsActive { get; set; } = true;// Aktif mi?
}
