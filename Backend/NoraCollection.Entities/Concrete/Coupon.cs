using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class Coupon : BaseEntity, IEntity
{
    public string? Code { get; set; }// Kupon kodu
    public decimal DiscountAmount { get; set; }//kuponun sağladığı indirim miktarı
    public DateTime ExpiryDate { get; set; }//Kuponun geçerlilik bitiş tarihi
}
