using Microsoft.AspNetCore.Http;

namespace NoraCollection.Shared.Dtos.HeroBannerDtos;

public class HeroBannerCreateDto
{
    public string Title { get; set; } = null!;
    public string? Subtitle { get; set; }
    public IFormFile Image { get; set; } = null!;
    public IFormFile? MobileImage { get; set; }
    public string? ButtonText { get; set; }
    public string? ButtonUrl { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}











