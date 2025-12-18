using System;

namespace NoraCollection.Shared.Dtos.SiteSettingDtos;

public class SiteSettingDto
{
    public int Id { get; set; }
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string? Description { get; set; }
    public string? Group { get; set; }
}
