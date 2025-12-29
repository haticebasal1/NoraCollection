using System;
using NoraCollection.Shared.Dtos.ProductImageDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IProductImageService
{
    // Belirli bir ürünün tüm aktif (silinmemiş) görsellerini listeler.
    Task<ResponseDto<IEnumerable<ProductImageDto>>> GetByProductIdAsync(int productId);
    // Ürüne yeni bir resim ekler.
    Task<ResponseDto<ProductImageDto>> AddAsync(ProductImageCreateDto productImageCreateDto);
    // Tek bir resmin detaylarını getirir.
    Task<ResponseDto<ProductImageDto>> GetAsync(int id);
    Task<ResponseDto<NoContentDto>> UpdateAsync(ProductImageUpdateDto productImageUpdateDto);
    // Bir resmi "Ana Görsel" (Kapak Fotoğrafı) yapar.
    Task<ResponseDto<NoContentDto>> SetAsMainAsync(int id);
    Task<ResponseDto<NoContentDto>> SoftDeleteAsync(int id);
    Task<ResponseDto<NoContentDto>> HardDeleteAsync(int id);
    // Resmin görüntülenme sırasını (DisplayOrder) değiştirir.
    Task<ResponseDto<NoContentDto>> UpdateDisplayOrderAsync(int id, int displayOrder);
}
