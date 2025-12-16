using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.ShippingDtos;

public class ShippingUpdateDto
{
    public string? TrackingNumber { get; set; }
    public string? Carrier { get; set; }

    [Required]
    public DateTime ShippingDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
}
