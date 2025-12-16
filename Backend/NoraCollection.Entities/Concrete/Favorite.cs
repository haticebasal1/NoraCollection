using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class Favorite : BaseEntity, IEntity
{
    public Favorite(string? userId, int productId, string? productName, decimal price, string? imageUrl)
    {
        UserId = userId;
        ProductId = productId;
        ProductName = productName;
        Price = price;
        ImageUrl = imageUrl;
    }

    private Favorite() { }
    public string? UserId { get; set; }
    public int ProductId { get; set; } 
    public User? User { get; set; }
    public Product? Product { get; set; }
    public string? ProductName { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}
