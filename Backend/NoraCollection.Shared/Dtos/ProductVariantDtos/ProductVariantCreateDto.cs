using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.ProductVariantDtos;

public class ProductVariantCreateDto
{
    [Required]
    public int ProductId { get; set; }

    [Required(ErrorMessage ="Beden/ölçü zorunludur!")]
    public string Size { get; set; } = null!;
    public string? SKU { get; set; }
    public int Stock { get; set; } = 0;
    public decimal? PriceAdjustment { get; set; }
}

