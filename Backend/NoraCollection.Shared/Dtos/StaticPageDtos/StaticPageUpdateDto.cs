namespace NoraCollection.Shared.Dtos.StaticPageDtos;

public class StaticPageUpdateDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public bool IsPublished { get; set; }
    public int DisplayOrder { get; set; }
}




