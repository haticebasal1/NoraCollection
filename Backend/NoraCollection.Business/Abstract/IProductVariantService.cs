using NoraCollection.Shared.Dtos.ProductVariantDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IProductVariantService
{
    // Belirli bir ürünün tüm varyantlarını getirir.
    Task<ResponseDto<IEnumerable<ProductVariantDto>>> GetByProductIdAsync(int productId);
    // Belirli bir ürünün sadece satışta olan (IsAvailable = true) varyantlarını getirir.
    Task<ResponseDto<IEnumerable<ProductVariantDto>>> GetAvailableByProductIdAsync(int productId);
    // Tek bir varyantı getirir.
    Task<ResponseDto<ProductVariantDto>> GetAsync(int id);
    // SKU'ya göre varyant bulur (stok takibi için).
    Task<ResponseDto<ProductVariantDto>> GetBySkuAsync(string sku);
    Task<ResponseDto<ProductVariantDto>> AddAsync(ProductVariantCreateDto productVariantCreateDto);
    Task<ResponseDto<NoContentDto>> UpdateAsync(ProductVariantUpdateDto productVariantUpdateDto);
    Task<ResponseDto<NoContentDto>> SoftDeleteAsync(int id);
    Task<ResponseDto<NoContentDto>> HardDeleteAsync(int id);
    // Stok güncelleme (sipariş sonrası vs.)
    Task<ResponseDto<NoContentDto>> UpdateStockAsync(int id, int stock);
    // Varyantın satış durumunu günceller (IsAvailable).
    Task<ResponseDto<NoContentDto>> UpdateAvailabilityAsync(int id, bool isAvailable);
    // Varyantın stok durumunu kontrol eder.
    Task<ResponseDto<bool>> CheckStockAsync(int id, int requestedQuantity);
}
