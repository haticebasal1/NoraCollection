using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.ColorDtos;

public class ColorCreateDto
{
    [Required(ErrorMessage = "Renk adı zorunludur!")]
    public string Name { get; set; } = null!;
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Hex kodu # ile başlamalı ve 6 haneli olmalıdır! (Örn: #FFD700)")]
    public string HexCode { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
}













