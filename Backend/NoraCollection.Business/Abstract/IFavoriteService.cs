using System;
using NoraCollection.Shared.Dtos.FavoriteDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IFavoriteService
{
    Task<ResponseDto<FavoriteDto>> AddAsync(FavoriteCreateDto favoriteCreateDto); //ekleme
    Task<ResponseDto<NoContentDto>> RemoveAsync(int id, string userId); //çıkarma
    Task<ResponseDto<FavoriteDto>> GetByIdAsync(int id); //tek bir favori getirme 
    Task<ResponseDto<IEnumerable<FavoriteDto>>> GetByUserId(string userId); // tüm favori getirme
    Task<ResponseDto<bool>> IsInFavoritesAsync(string userId, int productId); //ürünün kullanıcının favorilerinde olup olmaması
}
