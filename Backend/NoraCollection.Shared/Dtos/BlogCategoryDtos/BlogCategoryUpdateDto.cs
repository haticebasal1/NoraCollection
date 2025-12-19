using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.BlogCategoryDtos;

public class BlogCategoryUpdateDto
{
    [Required]
    public int Id { get; set; }
    [Required(ErrorMessage = "Kategori adı zorunludur!")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Slug zorunludur!")]
    public string Slug { get; set; } = null!;
    public string? Description { get; set; }
}


