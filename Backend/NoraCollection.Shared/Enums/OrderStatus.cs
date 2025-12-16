using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Enums;

public enum OrderStatus
{
    [Display(Name = "Sipariş Alındı")]
    Pending = 0,
    [Display(Name = "Hazırlanıyor")]
    Proccessing = 1,
    [Display(Name = "Kargoya Verildi")]
    Shipped = 2,
    [Display(Name = "Teslim Edildi")]
    Delivered = 3
}
