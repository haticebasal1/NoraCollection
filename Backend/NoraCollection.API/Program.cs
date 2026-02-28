using Microsoft.EntityFrameworkCore;
using NoraCollection.Shared.Configurations.Auth;
using NoraCollection.Business.Abstract;
using NoraCollection.Business.Concrete;
using NoraCollection.Business.Mappings;
using NoraCollection.Data.Abstract;
using NoraCollection.Data.Concrete;
using NoraCollection.Data.Concrete.Repositories;
using NoraCollection.Shared.Configurations;
using NoraCollection.Shared.Configurations.Email;
using NoraCollection.Entities.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NoraCollection.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true; // Şifrede en az 1 rakam
    options.Password.RequireLowercase = true; // En az 1 küçük harf
    options.Password.RequireUppercase = true; // En az 1 büyük harf
    options.Password.RequiredLength = 6; // En az 6 karakter
})
.AddEntityFrameworkStores<AppDbContext>() //Kullanıcı bilgisi db'de
.AddDefaultTokenProviders(); //Reset token vb. için

var jwtConfig = builder.Configuration["JwtConfig:Secret"]
       ?? throw new InvalidOperationException("JwtConfig:Secret appsettings'te tanımlanmalı!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
        ValidAudience = builder.Configuration["JwtConfig:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig)),
        ClockSkew = TimeSpan.Zero //Token süresi hassas olsun
    };
});

builder.Services.AddControllers();
//Tarayıcı, varsayılan olarak frontend (örn. localhost:3000) ile backend (örn. localhost:5000) farklı port/domainde olduğunda isteği engeller. CORS ile backend, hangi origin’lere izin vereceğini belirtir.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:3000", "https://localhost:5173", "http://localhost:3000", "http://localhost:5173")
             .AllowAnyHeader()
             .AllowAnyMethod();
    });
});
builder.Services.AddAutoMapper(typeof(CategoryProfile).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<IImageService, ImageManager>();
builder.Services.AddScoped<IProductImageService, ProductImageManager>();
builder.Services.AddScoped<IProductVariantService, ProductVariantManager>();
builder.Services.AddScoped<IProductService, ProductManager>();
builder.Services.AddScoped<IStoneTypeService, StoneTypeManager>();
builder.Services.AddScoped<IColorService, ColorManager>();
builder.Services.AddScoped<IHeroBannerService, HeroBannerManager>();
builder.Services.AddScoped<ICartService, CartManager>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderManager>();
builder.Services.AddScoped<IAuthService, AuthManager>();
builder.Services.AddScoped<IFavoriteService, FavoriteManager>();
builder.Services.AddScoped<IEmailService, EmailManager>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateServices>();

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("EmailConfig"));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// Ana sayfaya (/) gelen istekleri direkt /swagger'a yönlendir
app.MapGet("/", () => Results.Redirect("/swagger"))
   .ExcludeFromDescription(); // Bu satır Swagger listesinde görünmesini engeller

app.Run();
