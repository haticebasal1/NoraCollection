namespace NoraCollection.Shared.Dtos.ColorDtos;

public class ColorCreateDto
{
    public string Name { get; set; } = null!;
    public string HexCode { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
}






