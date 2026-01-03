using System;
using NoraCollection.Shared.Dtos.ProductImageDtos;
using NoraCollection.Shared.Dtos.ProductVariantDtos;

namespace NoraCollection.Shared.Dtos.ProductDtos;

public class ProductWithVariantsDto:ProductDto
{
    // Ürün detay sayfası için ek bilgiler (ProductDto'da olmayanlar)
    public string? Slug { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string? Description { get; set; }
    public string? StoneInfo { get; set; }
    public string? UsageSuggestion { get; set; }
    public string? DesignStory { get; set; }
    public bool IsBestSeller { get; set; }
    public bool IsNewArrival { get; set; }
    public bool IsFeatured { get; set; }
    
    // Filtre bilgileri
    public int? StoneTypeId { get; set; }
    public string? StoneTypeName { get; set; }
    public int? ColorId { get; set; }
    public string? ColorName { get; set; }
    public string? ColorHexCode { get; set; }
    
    // Ürünün tüm varyantları (bedenler, stoklar)
    public ICollection<ProductVariantDto> ProductVariants { get; set; } = [];
    
    // Ürünün tüm resimleri (galeri için)
    public ICollection<ProductImageDto> ProductImages { get; set; } = [];
}
