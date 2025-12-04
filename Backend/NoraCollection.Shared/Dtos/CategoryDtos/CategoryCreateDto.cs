using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace NoraCollection.Shared.Dtos.CategoryDtos;

public class CategoryCreateDto
{
    [Required(ErrorMessage = "Kategori adı zorunludur!")]
    [Display(Name = "Kategori")]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "Kategori açıklaması zorunludur!")]
    [Display(Name = "Açıklama")]
    public string Description { get; set; } = string.Empty;
    [Required(ErrorMessage = "Görsel zorunludur!")]
    [Display(Name = "Görsel")]
    public IFormFile Image { get; set; } = null!;
}
