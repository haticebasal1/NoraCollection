namespace NoraCollection.Shared.Dtos.ProductImageDtos;

public class ProductImageDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int? ColorId { get; set; }  // ✅ Renk ID'si
    public string ImageUrl { get; set; } = null!;
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
}













