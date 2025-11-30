using System;
using NoraCollection.Entities.Abstract;

namespace NoraCollection.Entities.Concrete;

public class ProductCategory:IEntity
{
    public ProductCategory(int productId, int categoryId)
    {
        ProductId = productId;
        CategoryId = categoryId;
    }

    private ProductCategory(){}
    
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
