using Microsoft.AspNetCore.Http;

namespace NoraCollection.Shared.Dtos.ProductImageDtos;

public class ProductImageCreateDto
{
    public int ProductId { get; set; }
    public IFormFile Image { get; set; } = null!;
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsMain { get; set; } = false;
}

