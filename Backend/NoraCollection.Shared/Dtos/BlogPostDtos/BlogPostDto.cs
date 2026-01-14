namespace NoraCollection.Shared.Dtos.BlogPostDtos;

public class BlogPostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Summary { get; set; }
    public string Content { get; set; } = null!;
    public string? FeaturedImageUrl { get; set; }
    
    // SEO
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    
    // Durum
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int ViewCount { get; set; }
    
    // İlişkiler
    public int BlogCategoryId { get; set; }
    public string? BlogCategoryName { get; set; }
    public string? AuthorName { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
}









