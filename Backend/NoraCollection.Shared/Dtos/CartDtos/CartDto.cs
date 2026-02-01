using System;
using NoraCollection.Shared.Dtos.AuthDtos;

namespace NoraCollection.Shared.Dtos.CartDtos;

public class CartDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public UserDto? User { get; set; }
    
    // Sepetteki her bir satırı tutan liste
    public ICollection<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();

    // --- Nora Collection Özel Alanları ---
    public string? GiftNote { get; set; }     // Kullanıcının yazdığı hediye notu
    public bool IsGiftPackage { get; set; }   // Hediye paketi seçildi mi?
    public int? GiftOptionId { get; set; }    // Özel kutu vb. seçeneği (Entity'deki karşılığı)
    public int? CouponId { get; set; }        // Uygulanan kupon
    public string? CouponCode { get; set; }   // Kupon kodu (göstermek için)
    public decimal CouponDiscountAmount { get; set; }  // Kuponun indirim tutarı

    // --- Hesaplanan Alanlar (Read-Only) ---
    
    // Sepet toplamı (Ürünler + Hediye paketi ücreti eklemek istersen buraya ekleyebilirsin)
    public decimal TotalAmount => CartItems.Sum(x => x.ItemTotal);

    // Header'da göstermek için toplam ürün adedi
    public int ItemsCount => CartItems.Sum(x => x.Quantity);
}