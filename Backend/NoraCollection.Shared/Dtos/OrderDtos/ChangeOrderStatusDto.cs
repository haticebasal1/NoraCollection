using System;
using System.ComponentModel.DataAnnotations;
using NoraCollection.Shared.Enums;

namespace NoraCollection.Shared.Dtos.OrderDtos;

public class ChangeOrderStatusDto
{
    [Required(ErrorMessage = "Sipariş id'si zorunludur!")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "Yeni Sipariş durumu bilgisi zorunludur!")]
    public OrderStatus OrderStatus { get; set; }
}
