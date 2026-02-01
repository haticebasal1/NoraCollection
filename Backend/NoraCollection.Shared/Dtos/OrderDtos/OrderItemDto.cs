using System;

namespace NoraCollection.Shared.Dtos.OrderDtos;

public class OrderItemDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal ItemAmount => Quantity * UnitPrice;
    public int? ProductVariantId { get; set; }
    public string? VariantName { get; set; }
}
