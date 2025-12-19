using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.ColorDtos;

public class ColorCreateDto
{
    [Required(ErrorMessage = "Renk adı zorunludur!")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Hex kodu zorunludur!")]
    public string HexCode { get; set; } = null!;
    public string? Slug { get; set; }
}


