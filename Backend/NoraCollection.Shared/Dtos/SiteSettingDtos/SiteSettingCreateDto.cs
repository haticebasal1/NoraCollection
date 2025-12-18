using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.SiteSettingDtos;

public class SiteSettingCreateDto
{
    [Required(ErrorMessage ="Key zorunludur!")]
    public string Key { get; set; } = null!;

    [Required(ErrorMessage ="Value zorunludur!")]
    public string Value { get; set; } = null!;
    public string? Description { get; set; }
    public string? Group { get; set; }
}
