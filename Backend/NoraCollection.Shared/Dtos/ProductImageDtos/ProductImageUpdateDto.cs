using Microsoft.AspNetCore.Http;

namespace NoraCollection.Shared.Dtos.ProductImageDtos;

public class ProductImageUpdateDto
{
    public int Id { get; set; }
    public int? ColorId { get; set; }  // ✅ Renk seçimi (opsiyonel)
    public IFormFile? Image { get; set; }
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
}













