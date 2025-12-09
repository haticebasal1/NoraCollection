using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.CustomDesignDtos;

public class CustomDesignCreateDto
{
    [Required(ErrorMessage = "Başlık zorunludur!")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Açıklama zorunludur!")]
    public string Description { get; set; } = null!;
    public string? ReferenceImageUrl { get; set; }
}
