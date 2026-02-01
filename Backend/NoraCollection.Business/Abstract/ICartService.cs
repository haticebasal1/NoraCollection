using System;
using NoraCollection.Shared.Dtos.CartDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface ICartService
{
    // Kullanıcı için yeni sepet oluşturur. UserId ile boş sepet veya ilk ürünlerle (CartItems) açılabilir.
   Task<ResponseDto<CartDto>> CreateCartAsync(CartCreateDto cartCreateDto);
   // Kullanıcının sepetini getirir: ürünler (varyant bilgisi, birim fiyat, toplam), hediye notu, hediye paketi seçimi, toplam tutar ve toplam kalem adedi.
   Task<ResponseDto<CartDto>> GetCartAsync(string userId);
   // Sepete ürün ekler. ProductId, ProductVariantId ve Quantity zorunlu; birim fiyat o anki Product/Variant fiyatından alınır ve sepette sabitlenir.
   Task<ResponseDto<CartItemDto>> AddToCartAsync(AddToCartDto addToCartDto);
   // Sepetten tek kalemi kaldırır (CartItemId ile).
   Task<ResponseDto<NoContentDto>> RemoveFromCartAsync (int cartItemId,string userId);
   // Kullanıcının sepetini tamamen boşaltır; hediye notu ve hediye paketi seçimi de sıfırlanabilir.
   Task<ResponseDto<NoContentDto>> ClearCartAsync(string userId);
   // Sepetteki bir kalemin adetini günceller (CartItemId + yeni Quantity).
   Task<ResponseDto<NoContentDto>> ChangeQuantityAsync(ChangeQuantityDto changeQuantityDto);
   // Sepet seviyesinde hediye notu ve hediye paketi seçimini günceller (checkout öncesi veya sepet sayfasında).
   Task<ResponseDto<NoContentDto>> UpdateCartOptionAsync(string userId,UpdateCartOptionsDto updateCartOptionsDto);
   // Header’daki sepet badge’i(yanında yazan adet) için toplam ürün adedini döner; tam sepet çekmeden sadece sayı.
   Task<ResponseDto<int>> GetCartItemsCountAsync(string userId);
   // Sepete kupon uygular (geçerli, aktif, süresi dolmamış ve kullanım limiti aşılmamış kupon).
   Task<ResponseDto<NoContentDto>> ApplyCouponAsync(string userId, string couponCode);
   // Sepetten kuponu kaldırır.
   Task<ResponseDto<NoContentDto>> RemoveCouponAsync(string userId);
}
