using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NoraCollection.Shared.Dtos.CartDtos;

public class ChangeQuantityDto
{
    [JsonIgnore] // Backend'de token'dan doldurulur; güvenlik için sepette sadece kendi kalemini güncelle
    public string? UserId { get; set; }

    [Required(ErrorMessage = "Hangi ürünün miktarının değişeceği belirtilmelidir!")]
    public int CartItemId { get; set; }

    [Required(ErrorMessage = "Miktar bilgisi zorunludur!")]
    [Range(0, 100, ErrorMessage = "Miktar 0 (sepetten çıkar) veya 1–100 olabilir.")]
    public int Quantity { get; set; }
}
