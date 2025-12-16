using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class Cart:BaseEntity,IEntity
{
    private Cart(){}
    public Cart(string? userId)
    {
        UserId = userId;
    }


 public string? UserId { get; set; }
 public User? User { get; set; }
 public ICollection<CartItem> CartItems { get; set; }=[];
}
