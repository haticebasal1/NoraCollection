using System;

namespace NoraCollection.Shared.Dtos.ShippingDtos;

public class ShippingDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Carrier { get; set; }
    public DateTime ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
}
