using System;

namespace NoraCollection.Shared.Dtos.StoneTypeDtos;

public class StoneTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }           
    public string? ImageUrl { get; set; }
}

