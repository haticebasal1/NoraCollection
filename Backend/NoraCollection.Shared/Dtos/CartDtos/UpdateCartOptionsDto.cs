using System;
namespace NoraCollection.Shared.Dtos.CartDtos;

public class UpdateCartOptionsDto
{
    // Siparişe eklenen hediye notu metni. Kullanıcı boş bırakabilir.
    public string? GiftNote { get; set; }
    // Hediye paketi (kutu/paket) istenip istenmediği. true ise paket ücreti sepete eklenir.
    public bool IsGiftPackage { get; set; }
    // Seçilen hediye/kutu seçeneğinin Id'si (GiftOption tablosu). Birden fazla paket seçeneği varsa hangisinin seçildiğini belirtir; null ise paket seçilmemiştir.
    public int? GiftOptionId { get; set; }
}
