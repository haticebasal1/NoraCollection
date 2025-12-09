using System;

namespace NoraCollection.Shared.Dtos.CouponDtos;

public class CouponDto
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public decimal DiscountRate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; }
}
     