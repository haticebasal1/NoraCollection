using System;
using NoraCollection.Shared.Enums;

namespace NoraCollection.Shared.Dtos.CustomDesignDtos;

public class CustomDesignDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ReferanceImageUrl { get; set; }
    public decimal? Price { get; set; }
    public CustomDesignStatus CustomDesignStatus { get; set; }
    public string? AdminNote { get; set; }
    public string? UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
