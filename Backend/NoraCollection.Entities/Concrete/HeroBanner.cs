using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class HeroBanner : BaseEntity, IEntity
{
    public HeroBanner(string title, string imageUrl, int displayOrder)
    {
        Title = title;
        ImageUrl = imageUrl;
        DisplayOrder = displayOrder;
    }

    private HeroBanner()
    {

    }
    public string Title { get; set; } = null!; // Başlık: "Doğal Taşlarda Modern Tasarım"
    public string? Subtitle { get; set; }// Alt başlık
    public string ImageUrl { get; set; } = null!;// Banner görseli
    public string? MobileImageUrl { get; set; }// Mobil için farklı görsel
    public string? ButtonText { get; set; } // Buton yazısı: "Koleksiyonları Keşfet"
    public string? ButtonUrl { get; set; }// Buton linki: "/koleksiyonlar"
    public int DisplayOrder { get; set; } = 0;// Sıralama
    public bool IsActive { get; set; } = true; // Aktif mi?
    public DateTime? StartDate { get; set; }// Başlangıç tarihi (kampanya için)
    public DateTime? EndDate { get; set; }  // Bitiş tarihi (kampanya için)

}
