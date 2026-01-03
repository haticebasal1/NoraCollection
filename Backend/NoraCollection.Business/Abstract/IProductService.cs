using System;
using NoraCollection.Shared.Dtos.ProductDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IProductService
{
  Task<ResponseDto<ProductDto>> GetAsync(int id, bool includeCategories = false);
  Task<ResponseDto<ProductWithVariantsDto>> GetWithVariantsByIdAsync(int id);
  Task<ResponseDto<ProductDto>> GetBySlugAsync(string slug,bool includeCategories = false);
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
Task<ResponseDto<IEnumerable<ProductDto>>> SearchAsync(string searchTerm);
  Task<ResponseDto<IEnumerable<ProductDto>>> GetAllDeletedAsync();
  Task<ResponseDto<IEnumerable<ProductDto>>> GetHomePageAsync(int? top = null);
  Task<ResponseDto<IEnumerable<ProductDto>>> GetBestSellersAsync(int? top = null);
  Task<ResponseDto<IEnumerable<ProductDto>>> GetNewArrivalsAsync(int? top = null);
  Task<ResponseDto<IEnumerable<ProductDto>>> GetFeaturedAsync(int? top = null);
  Task<ResponseDto<IEnumerable<ProductDto>>> GetSimilarAsync(int productId, int? top = null);
  Task<ResponseDto<IEnumerable<ProductDto>>> GetOnSaleAsync(int? top = null);
  Task<ResponseDto<int>> CountAsync(bool? isDeleted = null, int? categoryId = null);
  Task<ResponseDto<ProductDto>> AddAsync(ProductCreateDto productCreateDto);
  Task<ResponseDto<NoContentDto>> UpdateAsync(ProductUpdateDto productUpdateDto);
  Task<ResponseDto<NoContentDto>> HardDeleteAsync(int id);
  Task<ResponseDto<NoContentDto>> SoftDeleteAsync(int id);

  Task<ResponseDto<NoContentDto>> UpdateIsHomeAsync(int id);
  Task<ResponseDto<NoContentDto>> UpdateIsBestSellerAsync(int id);
  Task<ResponseDto<NoContentDto>> UpdateIsNewArrivalAsync(int id);
  Task<ResponseDto<NoContentDto>> UpdateIsFeaturedAsync(int id);
  Task<ResponseDto<NoContentDto>> UpdateStockAsync(int id, int stock);
  Task<ResponseDto<NoContentDto>> UpdateDiscountedPriceAsync(int id, decimal? discountedPrice);
  Task<ResponseDto<NoContentDto>> SoftDeleteByCategoryIdAsync(int categoryId);
}
