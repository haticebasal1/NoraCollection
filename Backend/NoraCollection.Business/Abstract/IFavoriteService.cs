using System;
using NoraCollection.Shared.Dtos.FavoriteDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IFavoriteService
{
    Task<ResponseDto<FavoriteDto>> AddAsync(string userId, int productId);// Favoriye Ekle/Çıkar
    Task<ResponseDto<NoContentDto>> RemoveAsync(string userId,int productId);
    Task<ResponseDto<bool>> ToggleAsync(string userId,int productId);// Toggle (Varsa çıkar, yoksa ekle) - Tek butonla
    Task<ResponseDto<IEnumerable<FavoriteDto>>> GetUserFavoritesAsync(string userId); // Favorilerimi Getir
    Task<ResponseDto<bool>> IsFavoriteAsync(string userId,int productId);// Bu ürün favorimde mi?
    Task<ResponseDto<int>> GetCountAsync(string userId);// Favori sayısı
}
