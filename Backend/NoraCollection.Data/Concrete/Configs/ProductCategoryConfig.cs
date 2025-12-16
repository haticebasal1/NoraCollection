using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoraCollection.Entities.Concrete;

namespace NoraCollection.Data.Concrete.Configs;

public class ProductCategoryConfig : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.HasKey(x => new { x.ProductId, x.CategoryId });
        List<ProductCategory> productCategories = [
           new (1,1),
           new (2,2),
           new (3,3),
           new (4,1),
           new (5,3)
    ];
    builder.HasData(productCategories);
    builder.HasQueryFilter(x=>!x.Category!.IsDeleted && !x.Product!.IsDeleted);//isdeleted=false
    }
}

