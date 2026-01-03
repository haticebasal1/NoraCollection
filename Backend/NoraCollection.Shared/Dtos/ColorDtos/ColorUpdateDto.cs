namespace NoraCollection.Shared.Dtos.ColorDtos;

public class ColorUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string HexCode { get; set; } = null!;
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}




