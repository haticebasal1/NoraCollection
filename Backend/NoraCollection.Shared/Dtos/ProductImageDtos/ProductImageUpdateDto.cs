using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.ProductImageDtos;

public class ProductImageUpdateDto
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Görsel URL zorunludur!")]
    public string ImageUrl { get; set; } = null!;
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsMain { get; set; } = false;
}


