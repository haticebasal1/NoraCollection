using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.BlogCategoryDtos;

public class BlogCategoryCreateDto
{
    [Required(ErrorMessage ="Kategori adı zorunludur!")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage ="Slug zorunludur!")]
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
}

