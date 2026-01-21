namespace NoraCollection.Shared.Dtos.StaticPageDtos;

public class StaticPageCreateDto
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public bool IsPublished { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
}










