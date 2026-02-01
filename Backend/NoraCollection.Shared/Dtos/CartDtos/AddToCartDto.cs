using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NoraCollection.Shared.Dtos.CartDtos;

public class AddToCartDto
{
    [JsonIgnore] // Frontend'den gönderilmez, Backend'de token'dan doldurulur
    public string? UserId { get; set; }

    [Required(ErrorMessage ="Ürün id bilgisi zorunludur!")]
    public int ProductId { get; set; }

    // KRİTİK: Seçilen varyant (Örn: Yüzük ölçüsü Id'si)
    [Required(ErrorMessage ="Lütfen ürün seçeneğini belirtiniz!")]
    public int ProductVariantId { get; set; }

    [Required(ErrorMessage ="Ürün miktarı zorunludur!")]
    [Range(1, 100, ErrorMessage ="Tek seferde en az 1, en fazla 100 adet ekleyebilirsiniz!")]
    public int Quantity { get; set; }
}