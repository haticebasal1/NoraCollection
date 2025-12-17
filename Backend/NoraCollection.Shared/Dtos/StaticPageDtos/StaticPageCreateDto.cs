using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.StaticPageDtos;

public class StaticPageCreateDto
{
    [Required(ErrorMessage = "Başlık zorunludur!")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage ="Slug zorunludur!")]
    public string Slug { get; set; } = null!;

    [Required(ErrorMessage = "İçerik zorunludur!")]
    public string Content { get; set; } = null!;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public bool IsPublished { get; set; }=true;
    public int DisplayOrder { get; set; }=0;
}
