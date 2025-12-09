using System;

namespace NoraCollection.Shared.Dtos.CouponDtos;

public class CouponUpdateDto
{
public decimal DiscountAmount { get; set; }
public DateTime ExpiryDate { get; set; }
public bool IsActive { get; set; }
}
