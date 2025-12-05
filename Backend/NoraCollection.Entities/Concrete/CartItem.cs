using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class CartItem:BaseEntity,IEntity
{
    public CartItem(int cartId, int productId, int quantity)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
    }

    private CartItem(){}
  public int CartId { get; set; }
  public Cart? Cart { get; set; }
  public int ProductId { get; set; }
  public Product? Product { get; set; }
  public int Quantity { get; set; }
  
}
