using System;

namespace NoraCollection.Shared.Dtos.OrderCouponDtos;

public class OrderCouponDto
{
 public int CouponId { get; set; }
 public string? CouponCode { get; set; }
 public decimal DiscountAmount { get; set; }
}
