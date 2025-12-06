using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NoraCollection.Shared.Dtos.CartDtos;

public class AddToCartDto
{
    [JsonIgnore]
    public string? UserId { get; set; }

    [Required(ErrorMessage ="Ürün id bilgisi zorunludur!")]
    [Range(1,int.MaxValue,ErrorMessage ="Geçersiz ürün Id!")]
    public int ProductId { get; set; }

    0
    public int Quantity { get; set; }
}
