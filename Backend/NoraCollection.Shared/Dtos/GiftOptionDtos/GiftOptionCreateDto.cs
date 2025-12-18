using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.GiftOptionDtos;

public class GiftOptionCreateDto
{
    [Required(ErrorMessage ="Ad zorunludur!")]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }=0;   
    public bool IsActive { get; set; }=true;
}
