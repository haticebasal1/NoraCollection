using System;
using System.Diagnostics.CodeAnalysis;
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
            if (!productCreateDto.Price.HasValue || productCreateDto.Price.Value <= 0)
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

    public async Task<ResponseDto<IEnumerable<ProductDto>>> GetAllAsync(bool includeCategories = false, int? categoryId = null, int? stoneTypeId = null, int? colorId = null, decimal? minPrice = null, decimal? maxPrice = null, string? orderBy = null)
    {
        try
        {
            // 1️⃣ TEMEL PREDICATE: Sadece silinmemiş ürünleri getir
            Expression<Func<Product, bool>> predicate = x => !x.IsDeleted;
            // 2️⃣ KATEGORİ FİLTRESİ: Eğer categoryId verilmişse, o kategorideki ürünleri filtrele
            if (categoryId.HasValue)
            {
                var categoryIdValue = categoryId.Value;
                predicate = CombinePredicates(predicate, x => x.ProductCategories.Any(pc => pc.CategoryId == categoryIdValue));
            }
            // 3️⃣ STONE TYPE FİLTRESİ: Eğer stoneTypeId verilmişse, o taş tipindeki ürünleri filtrele
            if (stoneTypeId.HasValue)
            {
                var stoneTypeIdValue = stoneTypeId.Value;
                predicate = CombinePredicates(predicate, x => x.StoneTypeId == stoneTypeIdValue);
            }
            // 4️⃣ COLOR FİLTRESİ: Eğer colorId verilmişse, o renkteki ürünleri filtrele
            if (colorId.HasValue)
            {
                var colorIdValue = colorId.Value;
                predicate = CombinePredicates(predicate, x => x.ColorId == colorIdValue);
            }
            // 5️⃣ MİNİMUM FİYAT FİLTRESİ: Eğer minPrice verilmişse, o fiyattan yüksek ürünleri filtrele
            if (minPrice.HasValue)
            {
                var minPriceValue = minPrice.Value;
                predicate = CombinePredicates(predicate, x => (x.DiscountedPrice ?? x.Price) >= minPriceValue);
            }
            // 6️⃣ MAKSİMUM FİYAT FİLTRESİ: Eğer maxPrice verilmişse, o fiyattan düşük ürünleri filtrele
            if (maxPrice.HasValue)
            {
                var maxPriceValue = maxPrice.Value;
                predicate = CombinePredicates(predicate, x => (x.DiscountedPrice ?? x.Price) <= maxPriceValue);
            }
            // 7️⃣ INCLUDE LİSTESİ: İlişkili tabloları bağlıyoruz
            var includeList = new List<Func<IQueryable<Product>, IQueryable<Product>>>();
            // StoneType ve Color her zaman include et (küçük veri, DTO'da boş kalmaması için)
            includeList.Add(
              query => query.Include(x => x.StoneType)
            );
            includeList.Add(
              query => query.Include(x => x.Color)
             );
            // Kategoriler sadece istenirse include et
            if (includeCategories)
            {
                includeList.Add(query => query.Include(x => x.ProductCategories).ThenInclude(y => y.Category));
            }
            // 8️⃣ ORDER BY: Sıralama mantığı
            Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderByFunc = null;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                orderBy = orderBy.ToLowerInvariant();
                orderByFunc = orderBy switch
                {
                    "price-asc" => query => query.OrderBy(x => x.DiscountedPrice ?? x.Price), // Fiyat: Düşükten Yükseğe
                    "price-desc" => query => query.OrderByDescending(x => x.DiscountedPrice ?? x.Price),// Fiyat: Yüksekten Düşüğe
                    "name-asc" => query => query.OrderBy(x => x.Name),// İsim: A-Z
                    "name-desc" => query => query.OrderByDescending(x => x.Name),// İsim: Z-A
                    "newest" => query => query.OrderByDescending(x => x.CreatedAt),// En Yeni
                    "oldest" => query => query.OrderBy(x => x.CreatedAt),// En Eski
                    _ => query => query.OrderByDescending(x => x.Id)// Varsayılan: Id'ye göre
                };
            }
            else
            {
                // Varsayılan sıralama: En yeni eklenenler en üstte (orderBy verilmezse)
                orderByFunc = query => query.OrderByDescending(x => x.CreatedAt);
            }
            // 9️⃣ REPOSITORY'DEN VERİ ÇEKME
            var products = await _productRepository.GetAllAsync(
               predicate: predicate,
               orderby: orderByFunc,
               includeDeleted: false,
               includes: includeList.ToArray()
            );
            // 🔟 MAPPING: Entity'leri DTO'lara dönüştür
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return ResponseDto<IEnumerable<ProductDto>>.Success(productDtos, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<ProductDto>>.Fail($"Beklenmedik Hata : {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<ProductDto>>> GetAllDeletedAsync()
    {
        try
        {
            // 1️⃣ Sadece silinmiş ürünler (Soft Delete)
            Expression<Func<Product, bool>> predicate = x => x.IsDeleted;
            // 2️⃣ Include listesi
            var includeList = new List<Func<IQueryable<Product>, IQueryable<Product>>>
            {
                query => query.Include(x=>x.StoneType),
                query => query.Include(x=>x.Color),
                query => query.Include(x=>x.ProductCategories)
                .ThenInclude(pc=>pc.Category),
            };
            // 3️⃣ Repository çağrısı
            // Silinmiş ürünlerde DeletedAt ile sıralamak daha mantıklı
            var products = await _productRepository.GetAllAsync(
                predicate: predicate,
                orderby: query => query.OrderByDescending(x => x.DeletedAt),
                includeDeleted: true,
                includes: includeList.ToArray()
            );

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return ResponseDto<IEnumerable<ProductDto>>.Success(productDtos, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<ProductDto>>.Fail($"Beklenmedik Hata : {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<ProductDto>> GetAsync(int id, bool includeCategories = false)
    {
        try
        {
            var includeList = new List<Func<IQueryable<Product>, IQueryable<Product>>>();

            includeList.Add(query => query.Include(x => x.StoneType));
            includeList.Add(query => query.Include(x => x.Color));
            if (includeCategories)
            {
                includeList.Add(query => query.Include(x => x.ProductCategories).ThenInclude(y => y.Category));
            }

            var product = await _productRepository.GetAsync(
                predicate: x => x.Id == id && !x.IsDeleted,
                includeDeleted: false,
                includes: includeList.ToArray()
            );

            if (product is null)
            {
                return ResponseDto<ProductDto>.Fail("Ürün bulunamadı!", StatusCodes.Status404NotFound);
            }

            var productDto = _mapper.Map<ProductDto>(product);
            return ResponseDto<ProductDto>.Success(productDto, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<ProductDto>.Fail($"Beklenmedik Hata : {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<ProductDto>>> GetBestSellersAsync(int? top = null)
    {
        try
        {
            // 1️⃣ Predicate: Sadece aktif ürünler
            Expression<Func<Product, bool>> predicate = x => !x.IsDeleted && x.IsBestSeller;
            // 2️⃣ Include listesi
            var includeList = new List<Func<IQueryable<Product>, IQueryable<Product>>>
         {
             query => query.Include(x=>x.StoneType),
             query => query.Include(x=>x.Color)
         };
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderByFunc = query => query.OrderByDescending(x => x.CreatedAt);
            // 3️⃣ Repository'den tüm aktif ürünleri çek
            var products = await _productRepository.GetAllAsync(
              predicate: predicate,
              top: top,
              orderby: orderByFunc,
              includeDeleted: false,
              includes: includeList.ToArray()
            );

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            return ResponseDto<IEnumerable<ProductDto>>.Success(productDtos, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<ProductDto>>.Fail($"Beklenmedik Hata : {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<ProductDto>>> GetByCategorySlugAsync(string categorySlug, int? stoneTypeId = null, int? colorId = null, decimal? minPrice = null, decimal? maxPrice = null)
    {
        try
        {
            // 1️⃣ KATEGORİ KONTROLÜ: Slug'a göre kategoriyi bul
            var category = await _categoryRepository.GetAsync(
             predicate: x => x.Slug == categorySlug && !x.IsDeleted
            );

            if (category is null)
            {
                return ResponseDto<IEnumerable<ProductDto>>.Fail($"{categorySlug} slug'ına sahip kategori bulunamadı!", StatusCodes.Status404NotFound);
            }
            // 2️⃣ TEMEL PREDICATE: Sadece silinmemiş ürünler ve belirtilen kategorideki ürünler
            Expression<Func<Product, bool>> predicate = x => !x.IsDeleted && x.ProductCategories.Any(pc => pc.CategoryId == category.Id);
            // 3️⃣ STONE TYPE FİLTRESİ
            if (stoneTypeId.HasValue)
            {
                var stoneTypeIdValue = stoneTypeId.Value;
                predicate = CombinePredicates(predicate, x => x.StoneTypeId == stoneTypeIdValue);
            }
            if (colorId.HasValue)
            {
                var colorIdValue = colorId.Value;
                predicate = CombinePredicates(predicate, x => x.ColorId == colorIdValue);
            }
            if (minPrice.HasValue)
            {
                var minPriceValue = minPrice.Value;
                predicate = CombinePredicates(predicate, x => (x.DiscountedPrice ?? x.Price) >= minPriceValue);
            }
            if (maxPrice.HasValue)
            {
                var maxPriceValue = maxPrice.Value;
                predicate = CombinePredicates(predicate, x => (x.DiscountedPrice ?? x.Price) <= maxPriceValue);
            }
            // 7️⃣ INCLUDE LİSTESİ: StoneType ve Color her zaman include et
            var includeList = new List<Func<IQueryable<Product>, IQueryable<Product>>>
            {
                query=>query.Include(x=>x.StoneType),
                query => query.Include(x=>x.Color)
            };
            // 8️⃣ SIRALAMA: Varsayılan olarak en yeni eklenenler en üstte
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderByFunc = query => query.OrderByDescending(x => x.CreatedAt);

            var products = await _productRepository.GetAllAsync(
             predicate: predicate,
             orderby: orderByFunc,
             includeDeleted: false,
             includes: includeList.ToArray()
            );
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return ResponseDto<IEnumerable<ProductDto>>.Success(productDtos, StatusCodes.Status200OK);

        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<ProductDto>>.Fail($"Beklenmedik Hata : {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<ProductDto>> GetBySlugAsync(string slug, bool includeCategories = false)
    {
        try
        {
            // 1️⃣ Include listesi: StoneType ve Color her zaman include et
            var includeList = new List<Func<IQueryable<Product>, IQueryable<Product>>>
            {
                query=>query.Include(x=>x.StoneType),
                query => query.Include(x=>x.Color)
            };
            // Kategoriler sadece istenirse include et
            if (includeCategories)
            {
                includeList.Add(query => query.Include(x => x.ProductCategories).ThenInclude(y => y.Category));
            }
            // 2️⃣ Repository çağrısı: Slug'a göre ürünü getir
            var product = await _productRepository.GetAsync(
                predicate: x => x.Slug == slug && !x.IsDeleted,
                includeDeleted: false,
                includes: includeList.ToArray()
            );
            if (product is null)
            {
                return ResponseDto<ProductDto>.Fail("Ürün bulunamadı!", StatusCodes.Status404NotFound);
            }

            var productDto = _mapper.Map<ProductDto>(product);
            return ResponseDto<ProductDto>.Success(productDto, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<ProductDto>.Fail($"Beklenmedik Hata : {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<ProductDto>>> GetFeaturedAsync(int? top = null)
    {
        try
        {
            Expression<Func<Product, bool>> predicate = x => !x.IsDeleted && x.IsFeatured;
            var includeList = new List<Func<IQueryable<Product>, IQueryable<Product>>>
            {
                query=>query.Include(x=>x.StoneType),
                query => query.Include(x=>x.Color)
            };
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderByFunc = query => query.OrderByDescending(x => x.CreatedAt);
            var products = await _productRepository.GetAllAsync(
                 predicate: predicate,
                 top: top,
                 orderby: orderByFunc,
                includeDeleted: false,
                includes: includeList.ToArray()
            );
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return ResponseDto<IEnumerable<ProductDto>>.Success(productDtos, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<ProductDto>>.Fail($"Öne çıkan ürünler getirilirken hata oluştu: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<ProductDto>>> GetHomePageAsync(int? top = null)
    {
        try
        {
            Expression<Func<Product, bool>> predicate = x => !x.IsDeleted && x.IsHome;
            var includeList = new List<Func<IQueryable<Product>, IQueryable<Product>>>
            {
                query=>query.Include(x=>x.StoneType),
                query => query.Include(x=>x.Color)
            };
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderByFunc = query => query.OrderByDescending(x => x.CreatedAt);
            var products = await _productRepository.GetAllAsync(
                 predicate: predicate,
                 top: top,
                 orderby: orderByFunc,
                includeDeleted: false,
                includes: includeList.ToArray()
            );
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return ResponseDto<IEnumerable<ProductDto>>.Success(productDtos, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<ProductDto>>.Fail($"Ana sayfa ürünleri getirilirken hata oluştu: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<ProductDto>>> GetNewArrivalsAsync(int? top = null)
    {
        try
        {
            Expression<Func<Product, bool>> predicate = x => !x.IsDeleted && x.IsNewArrival;
            var includeList = new List<Func<IQueryable<Product>, IQueryable<Product>>>
            {
                query=>query.Include(x=>x.StoneType),
                query => query.Include(x=>x.Color)
            };
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderByFunc = query => query.OrderByDescending(x => x.CreatedAt);
            var products = await _productRepository.GetAllAsync(
                 predicate: predicate,
                 top: top,
                 orderby: orderByFunc,
                includeDeleted: false,
                includes: includeList.ToArray()
            );
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return ResponseDto<IEnumerable<ProductDto>>.Success(productDtos, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<ProductDto>>.Fail($"Yeni eklenen ürünler getirilirken hata oluştu: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<ProductDto>>> GetOnSaleAsync(int? top = null)
    {
        try
        {
            Expression<Func<Product, bool>> predicate = x => !x.IsDeleted && x.DiscountedPrice != null;
            var includeList = new List<Func<IQueryable<Product>, IQueryable<Product>>>
            {
                query=>query.Include(x=>x.StoneType),
                query => query.Include(x=>x.Color)
            };
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderByFunc = query => query.OrderByDescending(x => x.CreatedAt);
            var products = await _productRepository.GetAllAsync(
                 predicate: predicate,
                 top: top,
                 orderby: orderByFunc,
                includeDeleted: false,
                includes: includeList.ToArray()
            );
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return ResponseDto<IEnumerable<ProductDto>>.Success(productDtos, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<ProductDto>>.Fail($"İndirimdeki ürünler getirilirken hata oluştu: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<ProductDto>>> GetSimilarAsync(int productId, int? top = null)
    {
        try
        {
            var product = await _productRepository.GetAsync(
                predicate: x => x.Id == productId && !x.IsDeleted,
                includeDeleted: false,
                includes: query => query
                     .Include(x => x.ProductCategories)
                     .Include(x => x.StoneType)
                     .Include(x => x.Color)
            );
            if (product is null)
            {
                return ResponseDto<IEnumerable<ProductDto>>.Fail("Ürün bulunamadı!", StatusCodes.Status404NotFound);
            }
            var categoryIds = product.ProductCategories.Select(pc => pc.CategoryId).ToList();
            Expression<Func<Product, bool>> predicate;
            if (categoryIds.Any())
            {
                var categoryIdsArray = categoryIds.ToArray();
                var stoneTypeIdValue = product.StoneTypeId;
                var colorIdValue = product.ColorId;

                predicate = x => !x.IsDeleted && x.Id != productId &&
                (
                   x.ProductCategories.Any(pc => categoryIdsArray.Contains(pc.CategoryId)) || (stoneTypeIdValue.HasValue && x.StoneTypeId == stoneTypeIdValue) || (colorIdValue.HasValue && x.ColorId == colorIdValue)
                   );
            }
            else
            {
                var stoneTypeIdValue = product.StoneTypeId;
                var colorIdValue = product.ColorId;

                predicate = x => !x.IsDeleted && x.Id != productId &&
                (
                    (stoneTypeIdValue.HasValue && x.StoneTypeId == stoneTypeIdValue) ||
                    (colorIdValue.HasValue && x.ColorId == colorIdValue)
                );
            }
            var includeList = new List<Func<IQueryable<Product>, IQueryable<Product>>>
            {
                query=>query.Include(x=>x.StoneType),
                query => query.Include(x=>x.Color)
            };
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderByFunc = query => query.OrderByDescending(x => x.CreatedAt);
            var similarProducts = await _productRepository.GetAllAsync(
                predicate: predicate,
                top: top,
                orderby: orderByFunc,
                includeDeleted: false,
                includes: includeList.ToArray()
            );
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(similarProducts);
            return ResponseDto<IEnumerable<ProductDto>>.Success(productDtos, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<ProductDto>>.Fail($"Benzer ürünler getirilirken hata oluştu: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<ProductWithVariantsDto>> GetWithVariantsByIdAsync(int id)
    {
        try
        {
            var includeList = new List<Func<IQueryable<Product>, IQueryable<Product>>>
            {
                query => query.Include(x=>x.ProductVariants),
                query=> query.Include(x=>x.ProductImages),
                query=> query.Include(x=>x.ProductCategories).ThenInclude(pc=>pc.Category),
                query => query.Include(x=>x.StoneType),
                query => query.Include(x=>x.Color)
            };
            var product = await _productRepository.GetAsync(
                predicate: x => x.Id == id && !x.IsDeleted,
                includeDeleted: false,
                includes: includeList.ToArray()
            );
            if (product is null)
            {
                return ResponseDto<ProductWithVariantsDto>.Fail("Ürün bulunamadı!", StatusCodes.Status404NotFound);
            }
            var productDtos = _mapper.Map<ProductWithVariantsDto>(product);
            return ResponseDto<ProductWithVariantsDto>.Success(productDtos, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<ProductWithVariantsDto>.Fail($"Ürün detayları getirilirken hata oluştu : {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> HardDeleteAsync(int id)
    {
        try
        {
            var product = await _productRepository.GetAsync(
                predicate: x => x.Id == id,
                includeDeleted: true,
                includes: query => query
                  .Include(x => x.ProductImages)
                  .Include(x => x.ProductVariants)
            );

            if (product is null)
            {
                return ResponseDto<NoContentDto>.Fail("Ürün bulunamadığı için silme işlemi gerçekleştirilemedi!!", StatusCodes.Status404NotFound);
            }
            if (!string.IsNullOrWhiteSpace(product.ImageUrl))
            {
                _imageManager.DeleteImage(product.ImageUrl);
            }
            if (product.ProductImages?.Any() == true)
            {
                foreach (var productImage in product.ProductImages!)
                {
                    if (!string.IsNullOrWhiteSpace(productImage.ImageUrl))
                    {
                        _imageManager.DeleteImage(productImage.ImageUrl);
                    }
                }
            }
            _productRepository.Delete(product);
            var result = await _unitOfWork.SaveAsync();

            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Ürün silinirken beklenmedik bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }

            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik Hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<ProductDto>>> SearchAsync(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return ResponseDto<IEnumerable<ProductDto>>.Fail("Arama terimi boş olamaz!", StatusCodes.Status400BadRequest);
            }
            var searchTermLower = searchTerm.Trim().ToLowerInvariant();
            Expression<Func<Product, bool>> predicate = x => !x.IsDeleted && (
                (x.Name != null && x.Name.ToLower().Contains(searchTermLower)) ||
                (x.Description != null && x.Description.ToLower().Contains(searchTermLower)));
            var includeList = new List<Func<IQueryable<Product>, IQueryable<Product>>>
            {
                query=>query.Include(x=>x.StoneType),
                query => query.Include(x=>x.Color)
            };
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderByFunc = query => query.OrderByDescending(x => x.CreatedAt);
            var products = await _productRepository.GetAllAsync(
                 predicate: predicate,
                 orderby: orderByFunc,
                includeDeleted: false,
                includes: includeList.ToArray()
            );
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return ResponseDto<IEnumerable<ProductDto>>.Success(productDtos, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<ProductDto>>.Fail($"Arama yapılırken hata oluştu: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> SoftDeleteAsync(int id)
    {
        try
        {
            var product = await _productRepository.GetAsync(
              predicate: x => x.Id == id && !x.IsDeleted,
              includeDeleted: false
            );
            if (product is null)
            {
                return ResponseDto<NoContentDto>.Fail("Ürün bulunamadığı için silme işlemi gerçekleştirilemedi!!", StatusCodes.Status404NotFound);
            }
            product.IsDeleted = true;
            product.DeletedAt = DateTimeOffset.UtcNow;
            _productRepository.Update(product);

            var result = await _unitOfWork.SaveAsync();

            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Ürün silinirken beklenmedik bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik Hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> SoftDeleteByCategoryIdAsync(int categoryId)
    {
        try
        {
            var category = await _categoryRepository.GetAsync(
                predicate: x => x.Id == categoryId && !x.IsDeleted,
                includeDeleted: false
            );
            if (category is null)
            {
                return ResponseDto<NoContentDto>.Fail($"{categoryId} id'li kategori bulunamadı!", StatusCodes.Status404NotFound);
            }
            var products = await _productRepository.GetAllAsync(
               predicate: x => !x.IsDeleted && x.ProductCategories.Any(y => y.CategoryId == categoryId),
               includeDeleted: false
           );
            if (!products.Any())
            {
                return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
            }
            foreach (var product in products)
            {
                product.IsDeleted = true;
                product.DeletedAt = DateTimeOffset.UtcNow;
            }
            _productRepository.BulkUpdate(products);
            var result = await _unitOfWork.SaveAsync();

            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Ürün silinirken beklenmedik bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }

            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);

        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik Hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> UpdateAsync(ProductUpdateDto productUpdateDto)
    {
        try
        {
            var product = await _productRepository.GetAsync(
                predicate: x => x.Id == productUpdateDto.Id && !x.IsDeleted,
                includeDeleted: false,
                includes: query => query.Include(x => x.ProductCategories)
            );
            if (product is null)
            {
                return ResponseDto<NoContentDto>.Fail($"{productUpdateDto.Id} id'li ürün bulunamadı!", StatusCodes.Status404NotFound);
            }
            if (productUpdateDto.CategoryIds == null || !productUpdateDto.CategoryIds.Any())
            {
                return ResponseDto<NoContentDto>.Fail("En az bir kategori seçilmelidir!", StatusCodes.Status400BadRequest);
            }
            var validCategories = await _categoryRepository.GetAllAsync(
                predicate: x => productUpdateDto.CategoryIds.Contains(x.Id) && !x.IsDeleted,
                includeDeleted: false
            );
            // Distinct() → Aynı kategori ID'si birden fazla gönderilmişse tek say
            if (validCategories.Count() != productUpdateDto.CategoryIds.Distinct().Count())
            {
                return ResponseDto<NoContentDto>.Fail("Seçilen kategorilerden bazıları geçersiz veya silinmiş!", StatusCodes.Status400BadRequest);
            }
            // Resim Yönetimi Ön Hazırlık
            // Eski resim URL'sini sakla (başarılı güncellemeden sonra silmek için)
            var oldImageUrl = product.ImageUrl;
            string? newImageUrl = null;
            // Yeni resim yüklendiyse
            if (productUpdateDto.Image is not null)
            {
                // Resmi yükle
                var imageUploadResult = await _imageManager.ResizeAndUploadAsync(
                    productUpdateDto.Image, "products"
                );
                if (!imageUploadResult.IsSuccessful)
                {
                    // Hata durumunda direkt dön (henüz veritabanı değişikliği yok)
                    return ResponseDto<NoContentDto>.Fail(imageUploadResult.Errors, imageUploadResult.StatusCode);
                }
                // Başarılı ise yeni URL'yi sakla
                newImageUrl = imageUploadResult.Data;
            }
            //SEO: İsim değiştiyse Slug'ı da güncelle
            if (product.Name != productUpdateDto.Name)
            {
                var slug = GenerateSlug(productUpdateDto.Name!);
                var originalSlug = slug;
                var counter = 1;
                // Mevcut ürünün ID'sini hariç tutarak slug benzersizliğini kontrol et
                while (await _productRepository.ExistsAsync(x => x.Slug == slug && x.Id != product.Id))
                {
                    slug = $"{originalSlug}-{counter}";
                    counter++;
                }
                product.Slug = slug;
            }
            // Mapping'den ÖNCE ImageUrl'i sakla
            var currentImageUrl = product.ImageUrl;
            _mapper.Map(productUpdateDto, product);
            // ImageUrl'i tekrar set et (güvenlik için)
            product.ImageUrl = currentImageUrl;
            // Yeni resim yüklendiyse güncelle
            if (newImageUrl is not null)
            {
                var baseUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
                product.ImageUrl = $"{baseUrl}/{newImageUrl.TrimStart('/')}";
            }
            // Önce mevcut kategorileri temizle
            product.ProductCategories.Clear();
            // Yeni kategorileri ekle
            foreach (var categoryId in productUpdateDto.CategoryIds.Distinct())
            {
                product.ProductCategories.Add(
                    new ProductCategory(product.Id, categoryId)
                );
            }
            product.UpdatedAt = DateTimeOffset.UtcNow;
            _productRepository.Update(product);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                if (newImageUrl is not null)
                {
                    _imageManager.DeleteImage(newImageUrl);
                }
                return ResponseDto<NoContentDto>.Fail("Beklenmedik bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            if (newImageUrl is not null && !string.IsNullOrWhiteSpace(oldImageUrl))
            {
                _imageManager.DeleteImage(oldImageUrl);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);

        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik Hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> UpdateDiscountedPriceAsync(int id, decimal? discountedPrice)
    {
        try
        {
            var product = await _productRepository.GetAsync(
               predicate: x => x.Id == id && !x.IsDeleted,
               includeDeleted: false
            );
            if (product is null)
            {
                return ResponseDto<NoContentDto>.Fail($"Ürün bulunamadığı için işlem gerçekleştirilemedi!", StatusCodes.Status404NotFound);
            }
            if (discountedPrice.HasValue)
            {
                if (discountedPrice.Value < 0)
                {
                    return ResponseDto<NoContentDto>.Fail("İndirimli fiyat negatif olamaz!", StatusCodes.Status400BadRequest);
                }
                // İndirimli fiyat, normal fiyattan büyük veya eşit olamaz
                if (discountedPrice.Value >= product.Price)
                {
                    return ResponseDto<NoContentDto>.Fail($"İndirimli fiyat({discountedPrice.Value:C}), normal fiyattan ({product.Price:C}) büyük veya eşit olamaz!", StatusCodes.Status400BadRequest);
                }
            }
            product.DiscountedPrice = discountedPrice;
            product.UpdatedAt = DateTimeOffset.UtcNow;

            _productRepository.Update(product);
            var result = await _unitOfWork.SaveAsync();

            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Güncelleme sırasında beklenmedik bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik Hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> UpdateIsBestSellerAsync(int id)
    {
        try
        {
            var product = await _productRepository.GetAsync(
               predicate: x => x.Id == id && !x.IsDeleted,
               includeDeleted: false
               );
            if (product is null)
            {
                return ResponseDto<NoContentDto>.Fail($"Ürün bulunamadığı için işlem gerçekleştirilemedi!", StatusCodes.Status404NotFound);
            }
            if (!product.IsBestSeller)
            {
                var bestSellerCount = await _productRepository.CountAsync(
                    predicate: x => x.IsBestSeller && !x.IsDeleted,
                    includeDeleted: false
                );
                if (bestSellerCount >= 10)
                {
                    return ResponseDto<NoContentDto>.Fail("En çok satanlar bölümünde en fazla 10 ürün gösterilebilir! Lütfen önce başka bir ürünün işaretini kaldırın!", StatusCodes.Status400BadRequest);
                }
            }
            product.IsBestSeller = !product.IsBestSeller;
            product.UpdatedAt = DateTimeOffset.UtcNow;

            _productRepository.Update(product);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Güncelleme sırasında beklenmedik bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik Hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> UpdateIsFeaturedAsync(int id)
    {
        try
        {
            var product = await _productRepository.GetAsync(
             predicate: x => x.Id == id && !x.IsDeleted,
             includeDeleted: false
            );
            if (product is null)
            {
                return ResponseDto<NoContentDto>.Fail($"Ürün bulunamadığı için işlem gerçekleştirilemedi!", StatusCodes.Status404NotFound);
            }
            if (!product.IsFeatured)
            {
                var featuredCount = await _productRepository.CountAsync(
                    predicate: x => x.IsFeatured && !x.IsDeleted,
                    includeDeleted: false
                );
                if (featuredCount >= 10)
                {
                    return ResponseDto<NoContentDto>.Fail("Öne çıkanlar bölümünde en fazla 10 ürün gösterilebilir! Lütfen önce başka bir ürünün işaretini kaldırın!", StatusCodes.Status400BadRequest);
                }
            }
            product.IsFeatured = !product.IsFeatured;
            product.UpdatedAt = DateTimeOffset.UtcNow;

            _productRepository.Update(product);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Güncelleme sırasında beklenmedik bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik Hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> UpdateIsHomeAsync(int id)
    {
        try
        {
            var product = await _productRepository.GetAsync(
                predicate: x => x.Id == id && !x.IsDeleted,
                includeDeleted: false
            );
            if (product is null)
            {
                return ResponseDto<NoContentDto>.Fail($"Ürün bulunamadığı için işlem gerçekleştirilemedi!", StatusCodes.Status404NotFound);
            }
            if (!product.IsHome)
            {
                // Mevcutta kaç ürün ana sayfada işaretli?
                var homeProductCount = await _productRepository.CountAsync(
                    predicate: x => x.IsHome && !x.IsDeleted,
                    includeDeleted: false
                );
                // Limit aşımı kontrolü (Max 10 ürün)
                if (homeProductCount >= 10)
                {
                    return ResponseDto<NoContentDto>.Fail("Ana sayfada en fazla 10 ürün gösterilebilir! Lütfen önce başka bir ürünün işaretini kaldırın", StatusCodes.Status400BadRequest);
                }
            }
            product.IsHome = !product.IsHome;
            product.UpdatedAt = DateTimeOffset.UtcNow;
            _productRepository.Update(product);
            var result = await _unitOfWork.SaveAsync();

            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Güncelleme sırasında beklenmedik bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);

        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik Hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> UpdateIsNewArrivalAsync(int id)
    {
        try
        {
            var product = await _productRepository.GetAsync(
             predicate: x => x.Id == id && !x.IsDeleted,
             includeDeleted: false
            );
            if (product is null)
            {
                return ResponseDto<NoContentDto>.Fail($"Ürün bulunamadığı için işlem gerçekleştirilemedi!", StatusCodes.Status404NotFound);
            }
            product.IsNewArrival = !product.IsNewArrival;
            product.UpdatedAt = DateTimeOffset.UtcNow;

            _productRepository.Update(product);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Güncelleme sırasında beklenmedik bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik Hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> UpdateStockAsync(int id, int stock)
    {
        try
        {
            var product = await _productRepository.GetAsync(
             predicate: x => x.Id == id && !x.IsDeleted,
             includeDeleted: false
            );
            if (product is null)
            {
                return ResponseDto<NoContentDto>.Fail($"Ürün bulunamadığı için işlem gerçekleştirilemedi!", StatusCodes.Status404NotFound);
            }
            if (stock < 0)
            {
                return ResponseDto<NoContentDto>.Fail("Stok miktarı negatif olamaz!", StatusCodes.Status400BadRequest);
            }
            product.Stock = stock;
            product.UpdatedAt = DateTimeOffset.UtcNow;

            _productRepository.Update(product);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Güncelleme sırasında beklenmedik bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik Hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
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
    // Helper Method: İki Expression'ı AND operatörü ile birleştirir
    // Bu metod, birden fazla filtreyi güvenli bir şekilde birleştirmek için kullanılır
    private Expression<Func<Product, bool>> CombinePredicates(
        Expression<Func<Product, bool>> first,
        Expression<Func<Product, bool>> second
    )
    {
        // Ortak bir parametre oluştur (her iki expression'da da "x" kullanılıyor)
        var parameter = Expression.Parameter(typeof(Product), "x");
        // İlk expression'daki parametreyi yeni parametreyle değiştir
        var leftVisitor = new ReplaceExpressionVisitor(first.Parameters[0], parameter);
        var left = leftVisitor.Visit(first.Body);
        // İkinci expression'daki parametreyi yeni parametreyle değiştir
        var rightVisitor = new ReplaceExpressionVisitor(second.Parameters[0], parameter);
        var right = rightVisitor.Visit(second.Body);
        // İki expression'ı AND operatörü ile birleştir
        return Expression.Lambda<Func<Product, bool>>(Expression.AndAlso(left!, right!), parameter);
    }
    // Expression'lardaki parametre çakışmasını önlemek için yardımcı class
    // Bu class, bir expression'daki parametreyi başka bir parametreyle değiştirir
    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _from;
        private readonly Expression _to;

        public ReplaceExpressionVisitor(Expression from, Expression to)
        {
            _from = from;
            _to = to;
        }
        public override Expression? Visit(Expression? node)
        {
            // Eğer ziyaret edilen node, değiştirilmesi gereken parametre ise, yeni parametreyi döndür
            // Değilse, normal ziyaret işlemini devam ettir
            return node == _from ? _to : base.Visit(node);
        }
    }
}

