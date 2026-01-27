namespace NoraCollection.Shared.Dtos.CampaignBarDtos;

public class CampaignBarUpdateDto
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public string? Icon { get; set; }
    public string? LinkUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

















