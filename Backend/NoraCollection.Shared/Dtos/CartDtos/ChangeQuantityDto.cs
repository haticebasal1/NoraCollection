using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.CartDtos;

public class ChangeQuantityDto
{
    [Required(ErrorMessage ="Cart Item id bilgisi zorunludur!")]
    public int CartItemId { get; set; }
    [Required(ErrorMessage ="Adet bilgisi zorunludur!")]
    public int Quantity { get; set; }
}
