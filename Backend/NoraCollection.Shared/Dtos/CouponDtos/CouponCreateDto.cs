using System;

namespace NoraCollection.Shared.Dtos.CouponDtos;

public class CouponCreateDto
{
    public string Code { get; set; } = null!;
    public decimal DiscountRate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public int UsageLimit { get; set; }
}
