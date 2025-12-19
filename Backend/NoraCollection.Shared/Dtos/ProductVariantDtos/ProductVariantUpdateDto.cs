using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.ProductVariantDtos;

public class ProductVariantUpdateDto
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage ="Beden/ölçü zorunludur!")]
    public string Size { get; set; } = null!;
    public string? SKU { get; set; }
    public int Stock { get; set; }
    public decimal? PriceAdjustment { get; set; }
}


