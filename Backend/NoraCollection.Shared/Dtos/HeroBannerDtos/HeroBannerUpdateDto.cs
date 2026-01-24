using Microsoft.AspNetCore.Http;

namespace NoraCollection.Shared.Dtos.HeroBannerDtos;

public class HeroBannerUpdateDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Subtitle { get; set; }
    public IFormFile? Image { get; set; }
    public IFormFile? MobileImage { get; set; }
    public string? ButtonText { get; set; }
    public string? ButtonUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}















