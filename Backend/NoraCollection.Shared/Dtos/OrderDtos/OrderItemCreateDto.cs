using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.OrderDtos;

public class OrderItemCreateDto
{
    [Required(ErrorMessage="Ürün id'si zorunludur!")]
    public int ProductId { get; set; }

    [Required(ErrorMessage ="Adet bilgisi zorunludur!")]
    [Range(1,int.MaxValue,ErrorMessage ="Adet en az 1 olmalıdır!")]
    public int Quantity { get; set; }

    [Required(ErrorMessage ="Ürün fiyatı zorunludur!")]
    public decimal UnitPrice { get; set; }
    public int? ProductVariantId { get; set; }
}
