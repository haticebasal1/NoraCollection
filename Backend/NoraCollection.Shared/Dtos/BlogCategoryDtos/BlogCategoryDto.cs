namespace NoraCollection.Shared.Dtos.BlogCategoryDtos;

public class BlogCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
    public int PostCount { get; set; }          // Bu kategorideki yazı sayısı
}

