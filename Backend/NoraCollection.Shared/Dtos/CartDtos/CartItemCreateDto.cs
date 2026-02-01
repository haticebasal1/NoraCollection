using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.CartDtos;

public class CartItemCreateDto
{
    [Required(ErrorMessage = "Ürün ID bilgisi zorunludur.")]
    public int ProductId { get; set; }

    // KRİTİK EKSİK: Varyant desteği
    [Required(ErrorMessage = "Lütfen ürün seçeneğini (varyant) belirtiniz.")]
    public int ProductVariantId { get; set; }

    [Required(ErrorMessage = "Miktar bilgisi zorunludur.")]
    [Range(1, 100, ErrorMessage = "Miktar en az 1, en fazla 100 olabilir.")]
    public int Quantity { get; set; }
}