using System;

namespace NoraCollection.Shared.Dtos.ColorDtos;

public class ColorDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string HexCode { get; set; } = null!;
    public string? Slug { get; set; }
}



