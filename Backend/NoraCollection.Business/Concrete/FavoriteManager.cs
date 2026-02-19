using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using NoraCollection.Business.Abstract;
using NoraCollection.Data.Abstract;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.FavoriteDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Concrete;

public class FavoriteManager : IFavoriteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<Favorite> _favoriteRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IMapper _mapper;

    public FavoriteManager(IUnitOfWork unitOfWork, IGenericRepository<Favorite> favoriteRepository, IGenericRepository<User> userRepository, IGenericRepository<Product> productRepository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _favoriteRepository = _unitOfWork.GetRepository<Favorite>();
        _userRepository = _unitOfWork.GetRepository<User>();
        _productRepository = _unitOfWork.GetRepository<Product>();
        _mapper = mapper;
    }

    public async Task<ResponseDto<FavoriteDto>> AddAsync(FavoriteCreateDto favoriteCreateDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(favoriteCreateDto.UserId))
            {
                return ResponseDto<FavoriteDto>.Fail("Kullanıcı kimliği boş olamaz!", StatusCodes.Status400BadRequest);
            }
            var isUserExists = await _userRepository.ExistsAsync(
                x => x.Id == favoriteCreateDto.UserId
            );
            if (!isUserExists)
            {
                return ResponseDto<FavoriteDto>.Fail("Kullanıcı bulunamadı!", StatusCodes.Status401Unauthorized);
            }
            var product = await _productRepository.GetAsync(
                x => x.Id == favoriteCreateDto.ProductId
            );
            if (product is null)
            {
                return ResponseDto<FavoriteDto>.Fail("Ürün bulunamadı!", StatusCodes.Status404NotFound);
            }
            var alReadyExists = await _favoriteRepository.GetAsync(
                x => x.UserId == favoriteCreateDto.UserId && x.ProductId == favoriteCreateDto.ProductId && !x.IsDeleted
            );
            if (alReadyExists != null)
            {
                return ResponseDto<FavoriteDto>.Fail("Bu ürün zaten favorilerinizde!", StatusCodes.Status400BadRequest);
            }
            var favorite = new Favorite
            (
                favoriteCreateDto.UserId!,
                product.Id,
                product.Name!,
                product.DiscountedPrice ?? product.Price,
                product.ImageUrl ?? ""
            );
            favorite.UpdatedAt = DateTimeOffset.UtcNow;
            await _favoriteRepository.AddAsync(favorite);
            var saveResult = await _unitOfWork.SaveAsync();
            if (saveResult < 1)
            {
                return ResponseDto<FavoriteDto>.Fail("Favori kaydedilirken bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            var result = _mapper.Map<FavoriteDto>(favorite);
            return ResponseDto<FavoriteDto>.Success(result, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return ResponseDto<FavoriteDto>.Fail($"Beklenmedik Hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public Task<ResponseDto<FavoriteDto>> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<IEnumerable<FavoriteDto>>> GetByUserId(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<bool>> IsInFavoritesAsync(string userId, int productId)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> RemoveAsync(int id, string userId)
    {
        throw new NotImplementedException();
    }
}
