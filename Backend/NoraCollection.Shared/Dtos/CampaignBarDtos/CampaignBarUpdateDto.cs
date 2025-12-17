using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.CampaignBarDtos;

public class CampaignBarUpdateDto
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Metin zorunludur")]
    public string Text { get; set; } = null!;
    
    public string? Icon { get; set; }
    public string? LinkUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
