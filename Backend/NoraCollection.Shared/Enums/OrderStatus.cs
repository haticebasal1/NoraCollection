using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Enums;

public enum OrderStatus
{

    // Sipariş alındı; ödeme tamamlanmış veya eski akışta kullanılıyor. Admin panelde "Sipariş Alındı" görünür.

    [Display(Name = "Sipariş Alındı")]
    Pending = 0,


    // Sipariş hazırlanıyor; paketleme aşamasında. Admin "Hazırlanıyor" olarak işaretler.

    [Display(Name = "Hazırlanıyor")]
    Proccessing = 1,


    // Sipariş kargoya verildi; takip numarası girilmiş. Müşteri kargo ile takip edebilir.

    [Display(Name = "Kargoya Verildi")]
    Shipped = 2,

    // Sipariş teslim edildi; müşteriye ulaştı. Sipariş süreci tamamlandı.
    [Display(Name = "Teslim Edildi")]
    Delivered = 3,


    // Ödeme bekleniyor. Sipariş oluşturuldu; henüz ödeme alınmadı.
    // Kredi kartı: Gateway yanıtı bekleniyor veya başarısız. Havale/EFT: Müşteri havale yapacak veya dekont yüklenecek.

    [Display(Name = "Ödeme Bekliyor")]
    PendingPayment = 10,


    // Ödeme alındı. Kart onaylandı veya havale/EFT admin tarafından onaylandı. Sipariş hazırlanmaya alınabilir.

    [Display(Name = "Ödendi")]
    Paid = 11,


    // Sipariş iptal edildi. Müşteri veya admin iptal etti; stok iade edilir.

    [Display(Name = "İptal")]
    Cancelled = 12
}
