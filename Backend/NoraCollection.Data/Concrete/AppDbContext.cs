using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NoraCollection.Entities.Concrete;

namespace NoraCollection.Data.Concrete;

public class AppDbContext:IdentityDbContext<User>
{
public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
{
}
public DbSet<Category> Categories { get; set; }
public DbSet<Product> Products { get; set; }
public DbSet<ProductCategory> ProductCategories { get; set; }
public DbSet<Favorite> Favorites { get; set; }
public DbSet<Cart> Carts { get; set; }
public DbSet<CartItem> CartItems { get; set; }
public DbSet<Order> Orders { get; set; }
public DbSet<OrderItem> OrderItems { get; set; }
public DbSet<Coupon> Coupons { get; set; }
public DbSet<OrderCoupon> OrderCoupons { get; set; }
public DbSet<Shipping> Shippings { get; set; }
public DbSet<CustomDesign> CustomDesigns { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
