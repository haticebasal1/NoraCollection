using Microsoft.EntityFrameworkCore;
using NoraCollection.Business.Abstract;
using NoraCollection.Business.Concrete;
using NoraCollection.Business.Mappings;
using NoraCollection.Data.Abstract;
using NoraCollection.Data.Concrete;
using NoraCollection.Data.Concrete.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
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

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
