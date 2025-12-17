using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class SiteSetting : BaseEntity, IEntity
{
    public SiteSetting(string key, string value)
    {
        Key = key;
        Value = value;
    }

    private SiteSetting()
    {

    }
    public string Key { get; set; } = null!; // Anahtar: "WhatsAppNumber", "InstagramUrl"
    public string Value { get; set; } = null!;// Değer: "+905551234567"
    public string? Description { get; set; }// Admin için açıklama
    public string? Group { get; set; }// Grup: "Contact", "Social", "Shipping"
}
