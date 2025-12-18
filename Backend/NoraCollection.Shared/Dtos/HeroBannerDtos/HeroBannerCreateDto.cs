using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.HeroBannerDtos;

public class HeroBannerCreateDto
{
    [Required(ErrorMessage = "Başlık zorunludur!")]
    public string Title { get; set; } = null!;

    public string? Subtitle { get; set; }

    [Required(ErrorMessage = "Görsel zorunludur!")]
    public string ImageUrl { get; set; } = null!;
    public string? MobileImageUrl { get; set; }
    public string? ButtonText { get; set; }
    public string? ButtonUrl { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

