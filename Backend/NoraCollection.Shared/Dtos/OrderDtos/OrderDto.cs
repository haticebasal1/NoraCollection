using System;
using System.Collections.Generic;
using NoraCollection.Shared.Dtos.AuthDtos;
using NoraCollection.Shared.Dtos.OrderCouponDtos;
using NoraCollection.Shared.Enums;

namespace NoraCollection.Shared.Dtos.OrderDtos;

public class OrderDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? CanceledDate { get; set; }
    public DateTime? OrderStatusUpdatedDate { get; set; }
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    public string? UserId { get; set; }
    public UserDto? User { get; set; }
    public string? CustomerName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? ZipCode { get; set; }
    public decimal TotalAmount { get; set; }     // Ürünlerin toplamı (indirimsiz)
    public decimal DiscountAmount { get; set; }  // Kupon indirimi
    public decimal ShippingFee { get; set; }     // Kargo ücreti
    public decimal FinalTotal { get; set; }      // Tahsil edilen toplam
    public ICollection<OrderItemDto> OrderItems { get; set; } = [];
    public ICollection<OrderCouponDto>? OrderCoupons { get; set; } = [];
    //  public ShippingDto? Shipping { get; set; }
}
