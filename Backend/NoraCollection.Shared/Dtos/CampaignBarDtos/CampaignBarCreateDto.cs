using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.CampaignBarDtos;

public class CampaignBarCreateDto
{
    [Required(ErrorMessage = "Metin zorunludur")]
    public string Text { get; set; } = null!;
    public string? Icon { get; set; }
    public string? LinkUrl { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}


