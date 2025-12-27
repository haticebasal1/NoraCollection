namespace NoraCollection.Shared.Dtos.ProductVariantDtos;

public class ProductVariantCreateDto
{
    public int ProductId { get; set; }
    public string Size { get; set; } = null!;
    public string? Sku { get; set; }
    public int Stock { get; set; } = 0;
    public decimal? PriceAdjustment { get; set; }
    public bool IsAvailable { get; set; } = true;
}

