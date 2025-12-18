using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.GiftOptionDtos;

public class GiftOptionUpdateDto
{
    [Required]
    public int Id { get; set; }
    
    [Required(ErrorMessage ="Ad zorunludur!")]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}
