using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.StoneTypeDtos;

public class StoneTypeCreateDto
{
    [Required(ErrorMessage ="Taş adı zorunludur!")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage ="Slug zorunludur!")]
    public string Slug { get; set; } = null!;  
    public string? Description { get; set; }  
     public string? ImageUrl { get; set; }  
}

