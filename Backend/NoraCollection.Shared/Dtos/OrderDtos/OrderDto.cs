using System;
using NoraCollection.Shared.Dtos.AuthDtos;
using NoraCollection.Shared.Enums;

namespace NoraCollection.Shared.Dtos.OrderDtos;

public class OrderDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? CanceledDate { get; set; }
    public DateTime? OrderStatusUpdateId { get; set; }
    public string? UserId { get; set; }
    public UserDto? User { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    public ICollection<OrderItemDto> OrderItems { get; set; } = [];
    public decimal TotalAmount => OrderItems.Sum(x => x.ItemAmount);
}
