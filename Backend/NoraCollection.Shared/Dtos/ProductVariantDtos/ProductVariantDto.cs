namespace NoraCollection.Shared.Dtos.ProductVariantDtos;

public class ProductVariantDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Size { get; set; } = null!;
    public int? ColorId { get; set; }  // ✅ Renk ID'si
    public string? Sku { get; set; }
    public int Stock { get; set; }
    public decimal? PriceAdjustment { get; set; }
    public bool IsAvailable { get; set; }
}













