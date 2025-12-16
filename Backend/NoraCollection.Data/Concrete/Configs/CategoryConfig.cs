using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoraCollection.Entities.Concrete;

namespace NoraCollection.Data.Concrete.Configs;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).IsRequired();
        builder.Property(x => x.ImageUrl).IsRequired();
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(200); ;
        builder.HasIndex(x => x.Slug).IsUnique();
        List<Category> categories = [
          new Category("Yüzük","Farklı taş ve modellerden oluşan yüzük koleksiyonu.","yuzuk.png","yuzuk-koleksiyonu"){Id=1},
          new Category("Kolye","Zarif ve modern kolye modelleri.","kolye.png","zarif-kolye-modelleri"){Id=2},
          new Category("Bileklik","Günlük ve özel tasarım bileklik çeşitleri.","bileklik.png","modern-bileklik-modelleri"){Id=3}
        ];
        builder.HasData(categories);
        builder.HasQueryFilter(x=>!x.IsDeleted);//isdeleted=false
    }
}
