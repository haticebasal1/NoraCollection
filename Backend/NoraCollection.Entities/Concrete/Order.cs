using System;
using System.Collections.Generic;
using NoraCollection.Entities.Abstract;
using NoraCollection.Shared.Enums;

namespace NoraCollection.Entities.Concrete;

public class Order : BaseEntity, IEntity
{
    // Finansal ve kritik iletişim bilgileri bu aşamada Servis Katmanı tarafından atanmalıdır.
    public Order(string? userId, string customerName, string phoneNumber, string? email,
                 string address, string city, string district, string? zipCode,
                 DateTime orderDate, decimal totalAmount, decimal discountAmount,
                 decimal shippingFee, decimal finalTotal)
    {
        UserId = userId;
        CustomerName = customerName;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        City = city;
        District = district;
        ZipCode = zipCode;
        OrderDate = orderDate;
        TotalAmount = totalAmount;
        DiscountAmount = discountAmount;
        ShippingFee = shippingFee;
        FinalTotal = finalTotal;
        // OrderStatus varsayılan olarak zaten Pending atanmıştır.
    }
    private Order() { }
    public string? UserId { get; set; }
    public User? User { get; set; }

    public string? CustomerName { get; set; } // Sipariş anındaki müşteri adı
    public string? PhoneNumber { get; set; }  // Sipariş anındaki telefon numarası
    public string? Email { get; set; }        // Sipariş anındaki e-posta
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? ZipCode { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }     // Ürünlerin İndirimsiz Toplamı
    public decimal DiscountAmount { get; set; }  // Uygulanan Toplam İndirim (Kuponlar vb.)
    public decimal ShippingFee { get; set; }     // Kargo Ücreti
    public decimal FinalTotal { get; set; }      // Müşteriden Tahsil Edilen Net Tutar
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    public ICollection<OrderItem> OrderItems { get; set; } = [];

    public ICollection<OrderCoupon>? OrderCoupons { get; set; } = [];

    public Shipping? Shipping { get; set; }
}