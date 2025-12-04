using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NoraCollection.Shared.Dtos.CategoryDtos;

public class CategoryUpdateDto
{
    [Required(ErrorMessage = "Id bilgisi zorunludur!")]
    [Display(Name = "Id")]

    public int Id { get; set; }

    [Required(ErrorMessage = "Kategori Adı zorunludur!")]
    [Display(Name = "Kategori")]

    public string? Name { get; set; }

    [Required(ErrorMessage = "Kategori açıklaması zorunludur!")]
    [Display(Name = "Açıklama")]

    public string? Description { get; set; }

    [Display(Name = "Kategori")]

    public IFormFile? Image { get; set; } = null!;
}
