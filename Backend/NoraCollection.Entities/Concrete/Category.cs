using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class Category : BaseEntity, IEntity
{
    private Category()
    {
    }
    public Category(string name, string? description, string? imageUrl, string slug)
    {
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        Slug = slug;
    }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string Slug { get; set; }
    public ICollection<ProductCategory> ProductCategories { get; set; } = [];
}