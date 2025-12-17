using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class CampaignBar : BaseEntity, IEntity
{
    public CampaignBar(string text, int displayOrder)
    {
        Text = text;
        DisplayOrder = displayOrder;
    }

    private CampaignBar()
    {
        
    }
    public string Text { get; set; } = null!; // Metin: "500 TL üzeri ücretsiz kargo"
    public string? Icon { get; set; } // İkon adı veya URL
    public string? LinkUrl { get; set; }// Tıklanabilir link
    public int DisplayOrder { get; set; } = 0;// Sıralama
    public bool IsActive { get; set; } = true; // Aktif mi?
    public DateTime? StartDate { get; set; }// Başlangıç tarihi
    public DateTime? EndDate { get; set; }  // Bitiş tarihi 
}
