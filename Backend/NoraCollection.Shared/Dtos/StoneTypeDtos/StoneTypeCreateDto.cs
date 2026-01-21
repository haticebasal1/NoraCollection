using Microsoft.AspNetCore.Http;

namespace NoraCollection.Shared.Dtos.StoneTypeDtos;

public class StoneTypeCreateDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public IFormFile? Image { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
}










