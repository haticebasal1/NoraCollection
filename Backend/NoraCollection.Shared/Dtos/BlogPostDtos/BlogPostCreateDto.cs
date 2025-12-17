using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.BlogPostDtos;

public class BlogPostCreateDto
{
    [Required(ErrorMessage = "Başlık zorunludur!")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Slug zorunludur!")]
    public string Slug { get; set; } = null!;
    public string? Summary { get; set; }

    [Required(ErrorMessage = "İçerik zorunludur!")]
    public string Content { get; set; } = null!;
    public string? FeaturedImageUrl { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }

    [Required]
    public int BlogCategoryId { get; set; }
    public bool IsPublished { get; set; } = false;
}
