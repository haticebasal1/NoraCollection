using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NoraCollection.Data.Concrete;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
       var optionsbuilder = new DbContextOptionsBuilder<AppDbContext>();
       optionsbuilder.UseSqlServer(
        "Server=localhost,1452;                             DataBase=NoraCollectionDb;User=sa;                                            Password=Asd123.,;Trust Server                    Certificate=true"
       );
       return new AppDbContext(optionsbuilder.Options);
    }
}
