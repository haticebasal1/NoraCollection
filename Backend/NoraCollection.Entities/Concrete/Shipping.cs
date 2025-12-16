using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class Shipping : BaseEntity, IEntity
{
    private Shipping() { }//Kargo bilgisi

    public Shipping(int orderId, string trackingNumber, string carrier, DateTime shippedDate)
    {
        OrderId = orderId;
        TrackingNumber = trackingNumber;
        Carrier = carrier;
        ShippedDate = shippedDate;
    }

    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public string? TrackingNumber { get; set; }//Kargo takip numarası
    public string? Carrier { get; set; }//Kargo şirketi
    public DateTime ShippedDate { get; set; }//Kargo gönderim tarihi
    public DateTime? DeliveredDate { get; set; } // teslim tarihi
}

