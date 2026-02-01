using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class Cart : BaseEntity, IEntity
{
    private Cart() { }
    public Cart(string? userId)
    {
        UserId = userId;
        IsActive = true; // Yeni oluşturulan sepet varsayılan olarak aktiftir
    }

    public string? UserId { get; set; }
    public User? User { get; set; }
    public ICollection<CartItem> CartItems { get; set; } = [];
    
    // Hediye ve Not Yönetimi
    public string? GiftNote { get; set; }
    public bool IsGiftPackage { get; set; } // Hediye paketi istiyor mu?
    public int? GiftOptionId { get; set; }
    public GiftOption? GiftOption { get; set; }

    // Sepet Durumu
    public bool IsActive { get; set; } // Sipariş tamamlanınca false'a çekilir
    // Kupon İlişkisi (İsteğe Bağlı)
    public int? CouponId { get; set; }
    public Coupon? Coupon { get; set; }
}
