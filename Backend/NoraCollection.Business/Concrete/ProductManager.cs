using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NoraCollection.Business.Abstract;
using NoraCollection.Data.Abstract;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.ProductDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Concrete;

public class ProductManager : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImageService _imageManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IGenericRepository<ProductCategory> _productCategoryRepository;
    private readonly IGenericRepository<ProductImage> _productImageRepository;
    private readonly IGenericRepository<ProductVariant> _productVariantRepository;
    private readonly IGenericRepository<StoneType> _stoneTypeRepository;
    private readonly IGenericRepository<Color> _colorRepository;

    public ProductManager(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageManager, IHttpContextAccessor httpContextAccessor, IGenericRepository<Product> productRepository, IGenericRepository<Category> categoryRepository, IGenericRepository<ProductCategory> productCategoryRepository, IGenericRepository<ProductImage> productImageRepository, IGenericRepository<ProductVariant> productVariantRepository, IGenericRepository<StoneType> stoneTypeRepository, IGenericRepository<Color> colorRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageManager = imageManager;
        _httpContextAccessor = httpContextAccessor;
        _productRepository = unitOfWork.GetRepository<Product>();
        _categoryRepository = unitOfWork.GetRepository<Category>();
        _productCategoryRepository = unitOfWork.GetRepository<ProductCategory>();
        _productImageRepository = unitOfWork.GetRepository<ProductImage>();
        _productVariantRepository = unitOfWork.GetRepository<ProductVariant>();
        _stoneTypeRepository = unitOfWork.GetRepository<StoneType>();
        _colorRepository = unitOfWork.GetRepository<Color>();
    }

    public async Task<ResponseDto<ProductDto>> AddAsync(ProductCreateDto productCreateDto)
    {
        try
        {
            // 1. Fiyat ve Kategori Ön Kontrolleri
            if (productCreateDto.Price <= 0)
            {
                return ResponseDto<ProductDto>.Fail("Ürün fiyatı 0'dan büyük olmalıdır!", StatusCodes.Status400BadRequest);
            }
            if (productCreateDto.CategoryIds == null || !productCreateDto.CategoryIds.Any())
            {
                return ResponseDto<ProductDto>.Fail("En az bir kategori seçilmelidir!", StatusCodes.Status400BadRequest);
            }
            // 2. Kategori Varlık Kontrolü
            foreach (var categoryId in productCreateDto.CategoryIds)
            {
                var isCategoryExists = await _categoryRepository.ExistsAsync(x => x.Id == categoryId && !x.IsDeleted);
                if (!isCategoryExists)
                {
                    return ResponseDto<ProductDto>.Fail($"{categoryId} Id'li kategori bulunamadı!", StatusCodes.Status400BadRequest);
                }
            }
            // 3. Resim Kontrolü ve Yükleme
            if (productCreateDto.Image is null)
            {
                return ResponseDto<ProductDto>.Fail("Ürün resmi zorunludur!", StatusCodes.Status400BadRequest);
            }
            var imageUploadResult = await _imageManager.ResizeAndUploadAsync(productCreateDto.Image, "products");
            if (!imageUploadResult.IsSuccessful)
            {
                return ResponseDto<ProductDto>.Fail(imageUploadResult.Errors, imageUploadResult.StatusCode);
            }
            // 4. Mapping ve Temel Bilgiler
            var product = _mapper.Map<Product>(productCreateDto);
            // 5. Slug Oluşturma ve Benzersizlik Kontrolü
            var slug = GenerateSlug(product.Name!);
            var originalSlug = slug;
            var counter = 1;
            while (await _productRepository.ExistsAsync(x => x.Slug == slug))
            {
                slug = $"{originalSlug}-{counter}";
                counter++;
            }
            product.Slug = slug;
            // 6. Resim URL'ini Tam Adres Olarak Belirle
            var baseUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            product.ImageUrl = $"{baseUrl}/{imageUploadResult.Data.TrimStart('/')}";
            // 7. Product'ı ÖNCE kaydet (ProductId'yi almak için)
            await _productRepository.AddAsync(product);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                _imageManager.DeleteImage(imageUploadResult.Data);
                return ResponseDto<ProductDto>.Fail("Ürün kaydedilirken teknik bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            // 8. ŞİMDİ ProductId var, ProductCategories'i ekle
            product.ProductCategories = productCreateDto.CategoryIds
            .Select(categoryId => new ProductCategory(product.Id, categoryId))
            .ToList();
            _productRepository.Update(product);
            result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                _imageManager.DeleteImage(imageUploadResult.Data);
                return ResponseDto<ProductDto>.Fail("Kategori ilişkileri kaydedilirken hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            // 9. DTO Dönüşü (İlişkili verileri de içermesi için tekrar çekiyoruz)
            var savedProduct = await _productRepository.GetAsync(
             predicate: x => x.Id == product.Id,
             includes: query => query.Include(x => x.ProductCategories).ThenInclude(y => y.Category)
            );
            var productDto = _mapper.Map<ProductDto>(savedProduct);
            return ResponseDto<ProductDto>.Success(productDto, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return ResponseDto<ProductDto>.Fail($"Beklenmedik Hata : {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<int>> CountAsync(bool? isDeleted = null, int? categoryId = null)
    {
        try
        {
            // 1. Temel kural: isDeleted null ise silinmemişleri getir, değilse gelen değeri kullan.
            bool deletedStatus = isDeleted ?? false;
            // 2. Başlangıç filtresi
            Expression<Func<Product, bool>> predicate = x => x.IsDeleted == deletedStatus;
             // 3. Kategori filtresi varsa, mevcut filtreyi bozmadan üzerine ekle
            if (categoryId.HasValue)
            {
                var categoryIdValue = categoryId.Value;
                predicate = x => x.IsDeleted == deletedStatus && x.ProductCategories.Any(pc => pc.CategoryId == categoryIdValue);
            }
            var count = await _productRepository.CountAsync(
                predicate: predicate,
                includeDeleted: isDeleted.HasValue
            );
            return ResponseDto<int>.Success(count, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<int>.Fail($"Beklenmedik Hata : {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public Task<ResponseDto<IEnumerable<ProductDto>>> GetAllAsync(bool includeCategories = false, int? categoryId = null, int? stoneTypeId = null, int? colorId = null, decimal? minPrice = null, decimal? maxPrice = null, string? orderBy = null)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<IEnumerable<ProductDto>>> GetAllDeletedAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<ProductDto>> GetAsync(int id, bool includeCategories = false)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<IEnumerable<ProductDto>>> GetBestSellersAsync(int? top = null)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<IEnumerable<ProductDto>>> GetByCategorySlugAsync(string categorySlug, int? stoneTypeId = null, int? colorId = null, decimal? minPrice = null, decimal? maxPrice = null)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<ProductDto>> GetBySlugAsync(string slug, bool includeCategories = false)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<IEnumerable<ProductDto>>> GetFeaturedAsync(int? top = null)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<IEnumerable<ProductDto>>> GetHomePageAsync(int? top = null)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<IEnumerable<ProductDto>>> GetNewArrivalsAsync(int? top = null)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<IEnumerable<ProductDto>>> GetOnSaleAsync(int? top = null)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<IEnumerable<ProductDto>>> GetSimilarAsync(int productId, int? top = null)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<ProductWithVariantsDto>> GetWithVariantsByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> HardDeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<IEnumerable<ProductDto>>> SearchAsync(string searchTerm)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> SoftDeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> SoftDeleteByCategoryIdAsync(int categoryId)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> UpdateAsync(ProductUpdateDto productUpdateDto)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> UpdateDiscountedPriceAsync(int id, decimal? discountedPrice)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> UpdateIsBestSellerAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> UpdateIsFeaturedAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> UpdateIsHomeAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> UpdateIsNewArrivalAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> UpdateStockAsync(int id, int stock)
    {
        throw new NotImplementedException();
    }
    private string GenerateSlug(string name)
    {
        var slug = name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("ı", "i")
            .Replace("ğ", "g")
            .Replace("ü", "u")
            .Replace("ş", "s")
            .Replace("ö", "o")
            .Replace("ç", "c");

        slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");
        slug = Regex.Replace(slug, @"-+", "-");
        slug = slug.Trim('-');

        return slug;
    }
}

