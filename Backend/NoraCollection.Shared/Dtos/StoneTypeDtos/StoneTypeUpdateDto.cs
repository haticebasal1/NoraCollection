using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NoraCollection.Shared.Dtos.StoneTypeDtos;

public class StoneTypeUpdateDto
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Taş adı zorunludur")]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
    public IFormFile? Image { get; set; }
}