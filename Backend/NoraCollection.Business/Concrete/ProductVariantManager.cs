using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using NoraCollection.Business.Abstract;
using NoraCollection.Data.Abstract;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.ProductVariantDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Concrete;

public class ProductVariantManager : IProductVariantService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<ProductVariant> _productVariantRepository;
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IGenericRepository<Color> _colorRepository;
    private readonly IMapper _mapper;

    public ProductVariantManager(IUnitOfWork unitOfWork, IGenericRepository<ProductVariant> productVariantRepository, IGenericRepository<Product> productRepository, IGenericRepository<Color> colorRepository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _productVariantRepository = _unitOfWork.GetRepository<ProductVariant>();
        _productRepository = _unitOfWork.GetRepository<Product>();
        _colorRepository = _unitOfWork.GetRepository<Color>();
        _mapper = mapper;
    }
    public async Task<ResponseDto<ProductVariantDto>> AddAsync(ProductVariantCreateDto productVariantCreateDto)
    {
        try
        {
            var productExists = await _productRepository.ExistsAsync(
                x => x.Id == productVariantCreateDto.ProductId && !x.IsDeleted
            );
            if (!productExists)
            {
                return ResponseDto<ProductVariantDto>.Fail("Ürün bulunamadı!", StatusCodes.Status404NotFound);
            }
            if (string.IsNullOrWhiteSpace(productVariantCreateDto.Size))
            {
                return ResponseDto<ProductVariantDto>.Fail("Beden bilgisi zorunludur!", StatusCodes.Status400BadRequest);
            }
            if (productVariantCreateDto.Stock < 0)
            {
                return ResponseDto<ProductVariantDto>.Fail("Stok miktarı negatif olamaz!", StatusCodes.Status400BadRequest);
            }
            
            // ✅ ColorId kontrolü (eğer ColorId varsa, Color'ın var olup olmadığını kontrol et)
            if (productVariantCreateDto.ColorId.HasValue)
            {
                var colorExists = await _colorRepository.ExistsAsync(
                    x => x.Id == productVariantCreateDto.ColorId.Value && !x.IsDeleted && x.IsActive
                );
                if (!colorExists)
                {
                    return ResponseDto<ProductVariantDto>.Fail("Seçilen renk bulunamadı veya aktif değil!", StatusCodes.Status404NotFound);
                }
            }
            
            // ✅ Duplicate kontrolü: Aynı ProductId + Size + ColorId kombinasyonu kontrolü
            var existingVariant = await _productVariantRepository.GetAsync(
                x => x.ProductId == productVariantCreateDto.ProductId 
                    && x.Size == productVariantCreateDto.Size 
                    && x.ColorId == productVariantCreateDto.ColorId 
                    && !x.IsDeleted
            );
            if (existingVariant is not null)
            {
                var colorInfo = productVariantCreateDto.ColorId.HasValue 
                    ? $" ve seçilen renkte" 
                    : "";
                return ResponseDto<ProductVariantDto>.Fail($"{productVariantCreateDto.Size} bedeninde{colorInfo} varyant zaten mevcut!", StatusCodes.Status400BadRequest);
            }
            var variantsMapper = _mapper.Map<ProductVariant>(productVariantCreateDto);
            await _productVariantRepository.AddAsync(variantsMapper);

            var result = await _unitOfWork.SaveAsync();

            if (result < 1)
            {
                return ResponseDto<ProductVariantDto>.Fail("Varyant kaydedilirken bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }

            var variantsDtoMapper = _mapper.Map<ProductVariantDto>(variantsMapper);
            return ResponseDto<ProductVariantDto>.Success(variantsDtoMapper, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return ResponseDto<ProductVariantDto>.Fail($"Hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<bool>> CheckStockAsync(int id, int requestedQuantity)
    {
        try
        {
            var variant = await _productVariantRepository.GetAsync(x => x.Id == id && !x.IsDeleted);
            if (variant is null)
            {
                return ResponseDto<bool>.Fail("Varyant bulunamadı!", StatusCodes.Status404NotFound);
            }
            if (requestedQuantity<0)
            {
                return ResponseDto<bool>.Fail("İstenen miktar negatif olamaz!",StatusCodes.Status400BadRequest);
            }
            bool hasEnoughStock = variant.Stock>= requestedQuantity;
            return ResponseDto<bool>.Success(hasEnoughStock,StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail($"Hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<ProductVariantDto>> GetAsync(int id)
    {
        try
        {
            var variant = await _productVariantRepository.GetAsync(x => x.Id == id && !x.IsDeleted);
            if (variant is null)
            {
                return ResponseDto<ProductVariantDto>.Fail("Varyant bulunamadı!", StatusCodes.Status404NotFound);
            }
            var variantsMapper = _mapper.Map<ProductVariantDto>(variant);
            return ResponseDto<ProductVariantDto>.Success(variantsMapper, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<ProductVariantDto>.Fail($"Hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<ProductVariantDto>>> GetAvailableByProductIdAsync(int productId)
    {
        try
        {
            var variants = await _productVariantRepository.GetAllAsync(
                x => x.ProductId == productId && !x.IsDeleted && x.IsAvailable && x.Stock > 0
            );
            if (variants is null || !variants.Any())
            {
                return ResponseDto<IEnumerable<ProductVariantDto>>.Fail("Bu ürün için şu an satın alınabilir bir seçenek bulunamadı!", StatusCodes.Status200OK);
            }
            var orderedVariants = variants.OrderBy(x => x.Size);
            var variantsMapper = _mapper.Map<IEnumerable<ProductVariantDto>>(orderedVariants);
            return ResponseDto<IEnumerable<ProductVariantDto>>.Success(variantsMapper, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<ProductVariantDto>>.Fail($"Hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<ProductVariantDto>>> GetByProductIdAsync(int productId)
    {
        try
        {
            var variants = await _productVariantRepository.GetAllAsync(x => x.ProductId == productId && !x.IsDeleted);
            if (variants == null || !variants.Any())
            {
                return ResponseDto<IEnumerable<ProductVariantDto>>.Fail("Bu ürüne ait tanımlı varyant bulunamadı!", StatusCodes.Status200OK);
            }
            var orderedVariants = variants?.OrderBy(x => x.Size);
            var variantsMapper = _mapper.Map<IEnumerable<ProductVariantDto>>(orderedVariants);
            return ResponseDto<IEnumerable<ProductVariantDto>>.Success(variantsMapper, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<ProductVariantDto>>.Fail($"Hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<ProductVariantDto>> GetBySkuAsync(string sku)
    {
        try
        {
            var variant = await _productVariantRepository.GetAsync(x => x.Sku == sku && !x.IsDeleted);
            if (variant is null)
            {
                return ResponseDto<ProductVariantDto>.Fail("Bu SKU ile kayıtlı ürün bulunamadı!", StatusCodes.Status404NotFound);
            }
            var variantsMapper = _mapper.Map<ProductVariantDto>(variant);
            return ResponseDto<ProductVariantDto>.Success(variantsMapper, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<ProductVariantDto>.Fail($"Hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> HardDeleteAsync(int id)
    {
        try
        {
            var variant = await _productVariantRepository.GetAsync(x => x.Id == id, includeDeleted: true);
            if (variant is null)
            {
                return ResponseDto<NoContentDto>.Fail("Varyant bulunamadı!", StatusCodes.Status404NotFound);
            }
            _productVariantRepository.Delete(variant);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Varyant silinirken bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> SoftDeleteAsync(int id)
    {
        try
        {
            var variant = await _productVariantRepository.GetAsync(x => x.Id == id && !x.IsDeleted);
            if (variant is null)
            {
                return ResponseDto<NoContentDto>.Fail("Varyant bulunamadı!", StatusCodes.Status404NotFound);
            }
            variant.IsDeleted = true;
            variant.DeletedAt = DateTimeOffset.UtcNow;
            _productVariantRepository.Update(variant);

            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Varyant silinirken bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> UpdateAsync(ProductVariantUpdateDto productVariantUpdateDto)
    {
        try
        {
            var variant = await _productVariantRepository.GetAsync(x => x.Id == productVariantUpdateDto.Id && !x.IsDeleted);
            if (variant is null)
            {
                return ResponseDto<NoContentDto>.Fail("Varyant bulunamadı!", StatusCodes.Status404NotFound);
            }
            // ✅ ColorId kontrolü (eğer ColorId varsa, Color'ın var olup olmadığını kontrol et)
            if (productVariantUpdateDto.ColorId.HasValue)
            {
                var colorExists = await _colorRepository.ExistsAsync(
                    x => x.Id == productVariantUpdateDto.ColorId.Value && !x.IsDeleted && x.IsActive
                );
                if (!colorExists)
                {
                    return ResponseDto<NoContentDto>.Fail("Seçilen renk bulunamadı veya aktif değil!", StatusCodes.Status404NotFound);
                }
            }
            
            // ✅ Duplicate kontrolü: Size veya ColorId değiştiyse, aynı kombinasyonun olup olmadığını kontrol et
            if (variant.Size != productVariantUpdateDto.Size || variant.ColorId != productVariantUpdateDto.ColorId)
            {
                var existingVariant = await _productVariantRepository.GetAsync(
                    x => x.ProductId == variant.ProductId 
                        && x.Size == productVariantUpdateDto.Size 
                        && x.ColorId == productVariantUpdateDto.ColorId 
                        && x.Id != productVariantUpdateDto.Id 
                        && !x.IsDeleted
                );
                if (existingVariant is not null)
                {
                    var colorInfo = productVariantUpdateDto.ColorId.HasValue 
                        ? $" ve seçilen renkte" 
                        : "";
                    return ResponseDto<NoContentDto>.Fail($"Bu ürün için '{productVariantUpdateDto.Size}' bedeninde{colorInfo} zaten başka bir varyant mevcut!", StatusCodes.Status400BadRequest);
                }
            }
            if (string.IsNullOrWhiteSpace(productVariantUpdateDto.Size))
            {
                return ResponseDto<NoContentDto>.Fail("Beden bilgisi zorunludur!", StatusCodes.Status400BadRequest);
            }
            if (productVariantUpdateDto.Stock < 0)
            {
                return ResponseDto<NoContentDto>.Fail("Stok miktarı negatif olamaz!", StatusCodes.Status400BadRequest);
            }
            _mapper.Map(productVariantUpdateDto, variant);
            variant.UpdatedAt = DateTimeOffset.UtcNow;
            _productVariantRepository.Update(variant);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Varyant güncellenirken bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> UpdateAvailabilityAsync(int id, bool isAvailable)
    {
        try
        {
            var variant = await _productVariantRepository.GetAsync(x => x.Id == id && !x.IsDeleted);
            if (variant is null)
            {
                return ResponseDto<NoContentDto>.Fail("Varyant bulunamadı!", StatusCodes.Status404NotFound);
            }
            variant.IsAvailable = isAvailable;
            variant.UpdatedAt = DateTimeOffset.UtcNow;
            _productVariantRepository.Update(variant);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Satış durumu güncellenirken bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> UpdateStockAsync(int id, int stock)
    {
        try
        {
            var variant = await _productVariantRepository.GetAsync(x => x.Id == id && !x.IsDeleted);
            if (variant is null)
            {
                return ResponseDto<NoContentDto>.Fail("Varyant bulunamadı!", StatusCodes.Status404NotFound);
            }
            if (stock < 0)
            {
                return ResponseDto<NoContentDto>.Fail("Stok miktarı negatif olamaz!", StatusCodes.Status400BadRequest);
            }
            variant.Stock = stock;
            variant.UpdatedAt = DateTimeOffset.UtcNow;
            _productVariantRepository.Update(variant);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Varyant güncellenirken bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }
}
