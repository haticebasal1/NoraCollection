using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoraCollection.Entities.Concrete;

namespace NoraCollection.Data.Concrete.Configs;

public class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Properties).IsRequired().HasMaxLength(200);
        builder.Property(X => X.ImageUrl).IsRequired();
        builder.Property(x => x.Price).HasColumnType("decimal(10,2)");
        builder.Property(x => x.IsHome).IsRequired();
        builder.Property(x => x.Stock).HasDefaultValue(0).IsRequired();
        builder.Ignore(x => x.IsInStock);
        List<Product> products = new List<Product>
            {
                new Product("Altın Yüzük", "22 ayar altın, zarif taşlı tasarım.", 1299, "products/yuzuk-altin.png", true){Id=1,IsDeleted=true},
                new Product("Gümüş Kolye", "925 ayar gümüş, inci detaylı.", 499, "products/kolye-gumus.png", false){Id=2},
                new Product("Derili Bileklik", "El yapımı deri, şık ve dayanıklı.", 299, "products/bileklik-deri.png", true){Id=3,IsDeleted=true},
                new Product("Sarı Altın Küpe", "Taşlı sarı altın küpe seti.", 349, "products/kupe-altin.png", false){Id=4},
                new Product("Gümüş Halhal", "Minimalist tasarım, gümüş zincir halhal.", 199, "products/halhal-gumus.png", true){Id=5,IsDeleted=true}
            };
            builder.HasData(products);
            builder.HasQueryFilter(x=>!x.IsDeleted);//isdeleted=false
    }
}
