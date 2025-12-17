using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NoraCollection.Data.Concrete.Configs;
using NoraCollection.Entities.Concrete;

namespace NoraCollection.Data.Concrete;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Ürün & Kategori
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }

    // Filtreler
    public DbSet<StoneType> StoneTypes { get; set; }
    public DbSet<Color> Colors { get; set; }

    // Sepet & Sipariş
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Shipping> Shippings { get; set; }

    // Kupon & İndirim
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<OrderCoupon> OrderCoupons { get; set; }

    // Kullanıcı İşlemleri
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<CustomDesign> CustomDesigns { get; set; }

    // Blog
    public DbSet<BlogCategory> BlogCategories { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }

    // Site Yönetimi
    public DbSet<HeroBanner> HeroBanners { get; set; }
    public DbSet<CampaignBar> CampaignBars { get; set; }
    public DbSet<StaticPage> StaticPages { get; set; }
    public DbSet<SiteSetting> SiteSettings { get; set; }
    public DbSet<GiftOption> GiftOptions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(CategoryConfig).Assembly);
        base.OnModelCreating(builder);
    }
}
