using System;
using NoraCollection.Entities.Abstract;
using NoraCollection.Shared.Enums;

namespace NoraCollection.Entities.Concrete;

public class CustomDesign : BaseEntity, IEntity
{
    public string? UserId { get; set; }
    public User? User { get; set; }

    public string Title { get; set; } = null!;          // Tasarım başlığı
    public string Description { get; set; } = null!;    // Kullanıcı açıklaması

    public string? ReferenceImageUrl { get; set; }       // Kullanıcı referans görseli

    public decimal? Price { get; set; }                  // Admin fiyatı
    public string? AdminNote { get; set; }               // Admin açıklaması

    public CustomDesignStatus CustomDesignStatus { get; set; } = CustomDesignStatus.Pending;// Pending / Approved / Completed
}
