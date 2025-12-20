using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.ProductImageDtos;

public class ProductImageCreateDto
{
    [Required]
    public int ProductId { get; set; }     

    [Required(ErrorMessage ="Görsel URL zorunludur!")]
    public string ImageUrl { get; set; } = null!;      
    public string? AltText { get; set; }                
    public int DisplayOrder { get; set; }=0;
    public bool IsMain { get; set; } =false;     
}



