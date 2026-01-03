namespace NoraCollection.Shared.Dtos.HeroBannerDtos;

public class HeroBannerDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Subtitle { get; set; }
    public string ImageUrl { get; set; } = null!;
    public string? MobileImageUrl { get; set; }
    public string? ButtonText { get; set; }
    public string? ButtonUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}




