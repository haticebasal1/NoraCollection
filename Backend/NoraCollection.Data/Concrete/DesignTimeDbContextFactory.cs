using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NoraCollection.Data.Concrete;

public class DesignTimeDbContextFactory 
    : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=localhost,1452;Database=NoraCollectionDb;User Id=sa;Password=Asd123.,;TrustServerCertificate=True;"
        );

        return new AppDbContext(optionsBuilder.Options);
    }
}
