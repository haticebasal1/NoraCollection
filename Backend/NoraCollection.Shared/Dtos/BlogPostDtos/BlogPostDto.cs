using System;
using NoraCollection.Shared.Dtos.BlogCategoryDtos;

namespace NoraCollection.Shared.Dtos.BlogPostDtos;

public class BlogPostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? Summary { get; set; } 
    public string Content { get; set; } = null!;
    public string? FeaturedImageUrl { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public bool IsPublished { get; set; } = false;
    public DateTime? PublishedAt { get; set; }  
    public int ViewCount { get; set; }
    public int BlogCategoryId { get; set; } 
    public BlogCategoryDto? BlogCategory { get; set; } = null!; 
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
