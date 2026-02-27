using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NoraCollection.Data.Concrete.Configs;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared;

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

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(CategoryConfig).Assembly);

        #region Users
        var hasher = new PasswordHasher<User>();
        var adminMember = new User("Hatice", "Başal", null, "Kadıköy", "İstanbul", Gender.Male, DateTimeOffset.UtcNow)
        {
            Id = "af7be952-7e42-4025-ae5b-efa3b3a9a728",
            UserName = "testadmin",
            NormalizedUserName = "TESTADMIN",
            Email = "testadmin@example.com",
            NormalizedEmail = "TESTADMIN@EXAMPLE.COM",
            EmailConfirmed = true
        };
        adminMember.PasswordHash = hasher.HashPassword(adminMember, "Qwe123.,");

        var userMember = new User("Esin", "Çelik", null, "Üsküdar", "İstanbul", Gender.Female, DateTimeOffset.UtcNow)
        {
            Id = "2535fc28-1f63-4389-837d-7c5c16bcbdea",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER",
            Email = "testuser@example.com",
            NormalizedEmail = "TESTUSER@EXAMPLE.COM",
            EmailConfirmed = true
        };
        userMember.PasswordHash = hasher.HashPassword(userMember, "Qwe123.,");

        builder.Entity<User>().HasData(adminMember, userMember);
        #endregion

        #region Roles
        var adminRole = new IdentityRole
        {
            Id = "1a401768-5c01-4c34-bf0d-547f0b8b4ab8",
            Name = "Admin",
            NormalizedName = "ADMIN"
        };
        var userRole = new IdentityRole
        {
            Id = "49afd76c-933f-4579-8346-d6c5d8b1a799",
            Name = "User",
            NormalizedName = "USER"
        };

        builder.Entity<IdentityRole>().HasData(adminRole, userRole);
        #endregion

        #region UserRoles
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { UserId = adminMember.Id, RoleId = adminRole.Id },
            new IdentityUserRole<string> { UserId = userMember.Id, RoleId = userRole.Id }
        );
        #endregion

        #region Carts
        builder.Entity<Cart>().HasQueryFilter(x => !x.IsDeleted);
        builder.Entity<CartItem>().HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<Cart>().HasData(
            new Cart(adminMember.Id) { Id = 1 },
            new Cart(userMember.Id) { Id = 2 }
        );
        #endregion

        #region Orders
        builder.Entity<Order>().HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId);
        #endregion

        base.OnModelCreating(builder);
    }
}
