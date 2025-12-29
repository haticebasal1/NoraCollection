using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using NoraCollection.Business.Abstract;
using NoraCollection.Data.Abstract;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.ProductImageDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Concrete;

public class ProductImageManager : IProductImageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<ProductImage> _productImageRepository;
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductImageManager(IUnitOfWork unitOfWork, IGenericRepository<ProductImage> productImageRepository, IGenericRepository<Product> productRepository, IMapper mapper, IImageService imageService, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _productImageRepository = _unitOfWork.GetRepository<ProductImage>();
        _productRepository = _unitOfWork.GetRepository<Product>();
        _mapper = mapper;
        _imageService = imageService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ResponseDto<ProductImageDto>> AddAsync(ProductImageCreateDto productImageCreateDto)
    {
        try
        {
            // 1. Product var mı kontrol et
            var productExists = await _productRepository.ExistsAsync(
               x => x.Id == productImageCreateDto.ProductId && !x.IsDeleted
            );
            if (!productExists)
            {
                return ResponseDto<ProductImageDto>.Fail("Ürün bulunamadı!", StatusCodes.Status404NotFound);
            }
            if (productImageCreateDto.Image is null || productImageCreateDto.Image.Length == 0)
            {
                return ResponseDto<ProductImageDto>.Fail("Resim dosyası zorunludur!", StatusCodes.Status400BadRequest);
            }
            // 2. Resim Yükleme İşlemi
            var imageResult = await _imageService.ResizeAndUploadAsync(productImageCreateDto.Image, "products");
            if (!imageResult.IsSuccessful)
            {
                return ResponseDto<ProductImageDto>.Fail(imageResult.Errors, imageResult.StatusCode);
            }
            // 3. Entity Oluşturma
            var productImage = _mapper.Map<ProductImage>(productImageCreateDto);

            var baseUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            productImage.ImageUrl = $"{baseUrl}/{imageResult.Data.TrimStart('/')}";
            // 4. Mantıksal Kontrol: Bu ürünün hiç resmi var mı?
            var existingImages = await _productImageRepository.GetAllAsync(
               x => x.ProductId == productImageCreateDto.ProductId && !x.IsDeleted
            );

            if (!existingImages.Any())
            {
                // İlk resim → Otomatik ana resim
                productImage.IsMain = true;
                productImage.DisplayOrder = 1;
            }
            else
            {
                // Sonraki resimler → DisplayOrder otomatik artır
                var maxOrder = existingImages.Max(x => x.DisplayOrder);
                productImage.DisplayOrder = maxOrder + 1;
            }
            await _productImageRepository.AddAsync(productImage);
            var result = await _unitOfWork.SaveAsync();

            if (result < 1)
            {
                // DB kaydı başarısızsa yüklenen resmi siliyoruz (Cleanup)
                _imageService.DeleteImage(productImage.ImageUrl);
                return ResponseDto<ProductImageDto>.Fail("Resim kaydedilirken bir hata oluştu.", StatusCodes.Status500InternalServerError);
            }
            var imageMapper = _mapper.Map<ProductImageDto>(productImage);
            return ResponseDto<ProductImageDto>.Success(imageMapper, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return ResponseDto<ProductImageDto>.Fail($"Hata : {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<ProductImageDto>> GetAsync(int id)
    {
        try
        {
            var image = await _productImageRepository.GetAsync(x => x.Id == id);
            if (image == null)
            {
                return ResponseDto<ProductImageDto>.Fail("Resim bulunamadı!", StatusCodes.Status404NotFound);
            }
            var imageMapper = _mapper.Map<ProductImageDto>(image);
            return ResponseDto<ProductImageDto>.Success(imageMapper, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<ProductImageDto>.Fail($"Hata : {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<ProductImageDto>>> GetByProductIdAsync(int productId)
    {
        try
        {
            var images = await _productImageRepository.GetAllAsync(x => x.ProductId == productId && !x.IsDeleted);
            if (images == null || !images.Any())
            {
                return ResponseDto<IEnumerable<ProductImageDto>>.Fail("Bu ürüne ait herhangi bir görsel bulunamadı!", StatusCodes.Status200OK);
            }

            var orderedImages = images.OrderBy(x => x.DisplayOrder).ThenByDescending(x => x.IsMain);
            var imagesMapper = _mapper.Map<IEnumerable<ProductImageDto>>(orderedImages);
            return ResponseDto<IEnumerable<ProductImageDto>>.Success(imagesMapper, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<ProductImageDto>>.Fail($"Hata : {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> HardDeleteAsync(int id)
    {
        try
        {
            // includeDeleted: true → Silinmiş resimleri de bulabilmek için
            var image = await _productImageRepository.GetAsync(
               predicate: x => x.Id == id,
               includeDeleted: true
            );
            if (image == null)
            {
                return ResponseDto<NoContentDto>.Fail("Silinecek resim bulunamadı!", StatusCodes.Status404NotFound);
            }
            // Eğer silinen resim "Ana Resim" ise, otomatik olarak başka bir resmi ana resim yap
            if (image.IsMain)
            {
                var remainingImages = await _productImageRepository.GetAllAsync(
                    x => x.ProductId == image.ProductId && x.Id != id && !x.IsDeleted
                );
                if (remainingImages.Any())
                {
                    var newMainImage = remainingImages.OrderBy(x => x.DisplayOrder).First();
                    newMainImage.IsMain = true;
                    _productImageRepository.Update(newMainImage);
                }
            }
            _productImageRepository.Delete(image);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Silme işlemi başarısız!", StatusCodes.Status500InternalServerError);
            }
            // DB'den silindiyse fiziksel dosyayı da sil
            if (!string.IsNullOrWhiteSpace(image.ImageUrl))
            {
                _imageService.DeleteImage(image.ImageUrl);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Hata : {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> SetAsMainAsync(int id)
    {
        try
        {
            // 1. Hedef resmi bul
            var targetImage = await _productImageRepository.GetAsync(x => x.Id == id);
            if (targetImage == null)
            {
                return ResponseDto<NoContentDto>.Fail("Görsel bulunamadı!", StatusCodes.Status404NotFound);
            }
            // 2. Ürüne ait diğer tüm aktif resimleri çek (silinmişleri hariç tut)
            var productImages = await _productImageRepository.GetAllAsync(
               x => x.ProductId == targetImage.ProductId && !x.IsDeleted
            );
            // 3. Döngü ile durumları güncelle
            foreach (var img in productImages)
            {
                // Eğer ID hedef resimse True, değilse False yap
                img.IsMain = (img.Id == id);
                _productImageRepository.Update(img);
            }
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Ana resim güncellenirken bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }

            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Hata : {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> SoftDeleteAsync(int id)
    {
        try
        {
            var image = await _productImageRepository.GetAsync(x=>x.Id == id);
            if (image == null)
            {
              return ResponseDto<NoContentDto>.Fail("Görsel bulunamadı!",StatusCodes.Status404NotFound);  
            }

            image.IsDeleted = true;
            image.DeletedAt = DateTimeOffset.UtcNow;
            // Eğer silinen ana resimse, IsMain'i false yap ve yeni ana resim seç
           if (image.IsMain)
           {
            image.IsMain = false;
            var remainingImages = await _productImageRepository.GetAllAsync(
                x=>x.ProductId == image.ProductId && x.Id != id && !x.IsDeleted
            );
            if (remainingImages.Any())
            {
                // En düşük DisplayOrder'a sahip aktif resmi ana resim yap
                var newMainImage = remainingImages.OrderBy(x=>x.DisplayOrder).First();
                newMainImage.IsMain = true;
                _productImageRepository.Update(newMainImage);
            }
           }

           _productImageRepository.Update(image);
           var result = await _unitOfWork.SaveAsync();

           if (result<1)
           {
            return ResponseDto<NoContentDto>.Fail("Görsel silinirken bir hata oluştu!",StatusCodes.Status500InternalServerError);
           }
           return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Hata : {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> UpdateAsync(ProductImageUpdateDto productImageUpdateDto)
    {
        try
        {
            var image = await _productImageRepository.GetAsync(x=>x.Id == productImageUpdateDto.Id);
            if (image == null)
            {
                return ResponseDto<NoContentDto>.Fail("Görsel bulunamadı!",StatusCodes.Status404NotFound);
            }
            var oldImageUrl = image.ImageUrl;
            string? newImageUrl = null;
            // Resim dosyası güncelleniyorsa
            if (productImageUpdateDto.Image is not null)
            {
                var imageUploadResult = await _imageService.ResizeAndUploadAsync(
                    productImageUpdateDto.Image,
                    "products"
                );
                if (!imageUploadResult.IsSuccessful)
                {
                    return ResponseDto<NoContentDto>.Fail(imageUploadResult.Errors,imageUploadResult.StatusCode);
                }
                var baseUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
                newImageUrl = $"{baseUrl}/{imageUploadResult.Data!.TrimStart('/')}";
            }
             // Metadata güncelleme (AltText, DisplayOrder, IsMain)
            _mapper.Map(productImageUpdateDto, image);
             // Yeni resim varsa URL'i güncelle
            if (newImageUrl is not null)
            {
                image.ImageUrl = newImageUrl;
            }
              // IsMain güncelleniyorsa, diğer resimlerin IsMain'ini false yap
            if (productImageUpdateDto.IsMain && !image.IsMain)
            {
                var productImages = await _productImageRepository.GetAllAsync(x=>x.ProductId == image.ProductId && x.Id != productImageUpdateDto.Id && !x.IsDeleted);
                foreach (var img in productImages)
                {
                    img.IsMain = false;
                    _productImageRepository.Update(img);
                }
            }
            image.UpdatedAt = DateTimeOffset.UtcNow;
            _productImageRepository.Update(image);

            var result = await _unitOfWork.SaveAsync();

            if (result<1)
            {
                // DB kaydı başarısızsa yeni yüklenen resmi sil
                if (newImageUrl is not null)
                {
                    _imageService.DeleteImage(newImageUrl);
                }
                return ResponseDto<NoContentDto>.Fail("Görsel güncellenirken bir hata oluştu!",StatusCodes.Status500InternalServerError);
            }
            // Eski resmi sil (yeni resim yüklendiyse)
            if (newImageUrl is not null && !string.IsNullOrWhiteSpace(oldImageUrl))
            {
                _imageService.DeleteImage(oldImageUrl);
            }

            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Hata : {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> UpdateDisplayOrderAsync(int id, int displayOrder)
    {
        try
        {
            var image = await _productImageRepository.GetAsync(x=>x.Id == id);
            if (image == null)
            {
                return ResponseDto<NoContentDto>.Fail("Görsel bulunamadı!",StatusCodes.Status404NotFound);
            }
            image.DisplayOrder = displayOrder;
            _productImageRepository.Update(image);

            var result = await _unitOfWork.SaveAsync();
            if (result<1)
            {
                return ResponseDto<NoContentDto>.Fail("Sıralama güncellenirken bir hata oluştu!",StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Hata : {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }
}
