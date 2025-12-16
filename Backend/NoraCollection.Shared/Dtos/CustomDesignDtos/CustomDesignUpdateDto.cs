using System;
using System.ComponentModel.DataAnnotations;
using NoraCollection.Shared.Enums;

namespace NoraCollection.Shared.Dtos.CustomDesignDtos;

public class CustomDesignUpdateDto
{
    [Required(ErrorMessage ="Tasarım id zorunludur!")]
    public int Id { get; set; }
    public decimal? Price { get; set; }
    public string? AdminNote { get; set; }

    [Required(ErrorMessage ="Tasarım durumu zorunludur!")]
    public CustomDesignStatus CustomDesignStatus { get; set; }
}
