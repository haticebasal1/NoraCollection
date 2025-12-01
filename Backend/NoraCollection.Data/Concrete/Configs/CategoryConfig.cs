using System;
using Microsoft.EntityFrameworkCore;
using NoraCollection.Entities.Concrete;

namespace NoraCollection.Data.Concrete.Configs;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Category> builder)
    {
        throw new NotImplementedException();
    }
}
