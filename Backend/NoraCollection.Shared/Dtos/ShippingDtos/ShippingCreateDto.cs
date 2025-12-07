using System;
using System.ComponentModel.DataAnnotations;

namespace NoraCollection.Shared.Dtos.ShippingDtos;

public class ShippingCreateDto
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    public string TrackingNumber { get; set; } = string.Empty;

    [Required]
    public string Carrier { get; set; } = string.Empty;

    [Required]
    public DateTime ShippedDate { get; set; }
}
