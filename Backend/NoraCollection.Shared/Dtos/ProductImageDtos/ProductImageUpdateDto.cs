using Microsoft.AspNetCore.Http;

namespace NoraCollection.Shared.Dtos.ProductImageDtos;

public class ProductImageUpdateDto
{
    public int Id { get; set; }
    public IFormFile? Image { get; set; }
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
}











