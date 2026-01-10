using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NoraCollection.Shared.Dtos.ProductDtos;

public class ProductUpdateDto
{
    [Required(ErrorMessage = "Id bilgisi zorunludur!")]
    public int Id { get; set; }  
    [Required(ErrorMessage = "Ürün adı zorunludur!")]
    public string? Name { get; set; }
    [Required(ErrorMessage = "Ürün özellikleri zorunludur!")]
    public string? Properties { get; set; }
    [Required(ErrorMessage = "Ürün fiyatı zorunludur!")]
    [Range(0.1, double.MaxValue, ErrorMessage ="Fiyatı 0'dan büyük olmalıdır!")]
    public decimal? Price { get; set; }
    public IFormFile? Image { get; set; }
    public bool IsHome { get; set; }
    [Required(ErrorMessage = "Stok miktarı zorunludur!")]
    [Range(0,int.MaxValue,ErrorMessage ="Stok negatif olamaz!")]
    public int Stock { get; set; }
    [Required(ErrorMessage = "En az bir kategori seçilmelidir!")]
    public ICollection<int> CategoryIds { get; set; } = [];
}
