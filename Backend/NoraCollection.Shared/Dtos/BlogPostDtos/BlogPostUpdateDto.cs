using Microsoft.AspNetCore.Http;

namespace NoraCollection.Shared.Dtos.BlogPostDtos;

public class BlogPostUpdateDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Summary { get; set; }
    public string Content { get; set; } = null!;
    public IFormFile? FeaturedImage { get; set; }
    
    // SEO
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    
    // Durum
    public bool IsPublished { get; set; }
    
    // İlişkiler
    public int BlogCategoryId { get; set; }
}














