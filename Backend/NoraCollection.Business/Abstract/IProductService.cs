using System;
using NoraCollection.Shared.Dtos.ProductDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IProductService
{
  Task<ResponseDto<ProductDto>> GetAsync(int id, bool includeCategories = false);
  //ID ile ürünü, tüm varyantlarını (bedenler, stoklar) ve görsellerini getirir.
  Task<ResponseDto<ProductWithVariantsDto>> GetWithVariantsByIdAsync(int id);
  // Slug ile tek bir ürün getirir (SEO için).
  Task<ResponseDto<ProductDto>> GetBySlugAsync(string slug, bool includeCategories = false);
  Task<ResponseDto<IEnumerable<ProductDto>>> GetAllAsync(
    bool includeCategories = false,
    int? categoryId = null,
    int? stoneTypeId = null,
    int? colorId = null,
    decimal? minPrice = null,
    decimal? maxPrice = null,
    string? orderBy = null
  );
  // Kategori slug'ı ile ürünleri getirir (SEO için önemli!)
  Task<ResponseDto<IEnumerable<ProductDto>>> GetByCategorySlugAsync(
      string categorySlug,
      int? stoneTypeId = null,
      int? colorId = null,
      decimal? minPrice = null,
      decimal? maxPrice = null
  );
  //Ürün adı veya açıklamasında arama yapar.
  Task<ResponseDto<IEnumerable<ProductDto>>> SearchAsync(string searchTerm);
  Task<ResponseDto<IEnumerable<ProductDto>>> GetAllDeletedAsync();
  //Ana sayfada gösterilecek ürünleri getirir (IsHome = true).
  Task<ResponseDto<IEnumerable<ProductDto>>> GetHomePageAsync(int? top = null);
  //En çok satan ürünleri getirir (IsBestSeller = true).
  Task<ResponseDto<IEnumerable<ProductDto>>> GetBestSellersAsync(int? top = null);
  //Yeni eklenen ürünleri getirir (IsNewArrival = true).
  Task<ResponseDto<IEnumerable<ProductDto>>> GetNewArrivalsAsync(int? top = null);
  //Öne çıkan ürünleri getirir (IsFeatured = true).
  Task<ResponseDto<IEnumerable<ProductDto>>> GetFeaturedAsync(int? top = null);
  //Belirli bir ürüne benzer ürünleri getirir (aynı kategori/taş tipi/renk).
  Task<ResponseDto<IEnumerable<ProductDto>>> GetSimilarAsync(int productId, int? top = null);
  //İndirimdeki ürünleri getirir (DiscountedPrice != null).
  Task<ResponseDto<IEnumerable<ProductDto>>> GetOnSaleAsync(int? top = null);
  Task<ResponseDto<int>> CountAsync(bool? isDeleted = null, int? categoryId = null);
  Task<ResponseDto<ProductDto>> AddAsync(ProductCreateDto productCreateDto);
  Task<ResponseDto<NoContentDto>> UpdateAsync(ProductUpdateDto productUpdateDto);
  Task<ResponseDto<NoContentDto>> HardDeleteAsync(int id);
  Task<ResponseDto<NoContentDto>> SoftDeleteAsync(int id);
  //Ürünün ana sayfa durumunu günceller (IsHome toggle).
  Task<ResponseDto<NoContentDto>> UpdateIsHomeAsync(int id);
  //Ürünün en çok satan durumunu günceller (IsBestSeller toggle).
  Task<ResponseDto<NoContentDto>> UpdateIsBestSellerAsync(int id);
  //Ürünün yeni eklenen durumunu günceller (IsNewArrival toggle).
  Task<ResponseDto<NoContentDto>> UpdateIsNewArrivalAsync(int id);
  //Ürünün öne çıkan durumunu günceller (IsFeatured toggle).
  Task<ResponseDto<NoContentDto>> UpdateIsFeaturedAsync(int id);
  //Ürünün stok miktarını günceller.
  Task<ResponseDto<NoContentDto>> UpdateStockAsync(int id, int stock);
  //Ürünün indirimli fiyatını günceller.
  Task<ResponseDto<NoContentDto>> UpdateDiscountedPriceAsync(int id, decimal? discountedPrice);
  //Belirli bir kategorideki tüm ürünleri yumuşak siler.
  Task<ResponseDto<NoContentDto>> SoftDeleteByCategoryIdAsync(int categoryId);
}
