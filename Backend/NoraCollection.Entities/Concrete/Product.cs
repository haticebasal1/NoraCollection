using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class Product : BaseEntity, IEntity
{
    public Product(string? name, string? properties, decimal price, string? imageUrl, bool isHome)
    {
        Name = name;
        Properties = properties;
        Price = price;
        ImageUrl = imageUrl;
        IsHome = isHome;
    }

    private Product() { }
    public string? Name { get; set; }
    public string? Properties { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsHome { get; set; }
    public int Stock { get; set; } = 0;          // veritabanında saklanır
    public bool IsInStock => Stock > 0;          // sadece okunur, kolay kullanım
    public ICollection<ProductCategory> ProductCategories { get; set; } = [];
}
