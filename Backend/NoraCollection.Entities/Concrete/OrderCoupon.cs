using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class OrderCoupon : BaseEntity, IEntity//Sipariş kuponu
{
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    
    public int CouponId { get; set; }
    public Coupon? Coupon { get; set; }
}
