using System.ComponentModel.DataAnnotations;
using NoraCollection.Shared.Enums;

namespace NoraCollection.Shared.Dtos.OrderDtos;

public class CheckoutDto
{
    [Required(ErrorMessage = "Ad soyad zorunludur!")]
    public string CustomerName { get; set; } = null!;

    [Required(ErrorMessage = "Telefon zorunludur!")]
    public string PhoneNumber { get; set; } = null!;

    [EmailAddress]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Adres zorunludur!")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "Şehir zorunludur!")]
    public string City { get; set; } = null!;

    [Required(ErrorMessage = "İlçe zorunludur!")]
    public string District { get; set; } = null!;

    public string? ZipCode { get; set; }

    public string? Note { get; set; }               // Sipariş notu
    public string? GiftNote { get; set; }           // Hediye notu
    public int? GiftOptionId { get; set; }          // Hediye paketi seçimi
    public string? CouponCode { get; set; }         // Kupon kodu
    public PaymentMethod PaymentMethod { get; set; }
}