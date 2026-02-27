using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

    public async Task<ResponseDto<FavoriteDto>> GetByIdAsync(int id)
    {
        try
        {
            var favorite = await _favoriteRepository.GetAsync(
                x => x.Id == id && !x.IsDeleted
            );
            if (favorite is null)
            {
                return ResponseDto<FavoriteDto>.Fail("Favori bulunamadı!", StatusCodes.Status404NotFound);
            }
            var result = _mapper.Map<FavoriteDto>(favorite);
            return ResponseDto<FavoriteDto>.Success(result, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<FavoriteDto>.Fail($"Beklenmedik Hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<FavoriteDto>>> GetByUserId(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ResponseDto<IEnumerable<FavoriteDto>>.Fail("Kullanıcı bilgisi bulunamadı!", StatusCodes.Status401Unauthorized);
            }
            var favorites = await _favoriteRepository.GetAllAsync(
                predicate: x => x.UserId == userId && !x.IsDeleted,
                includeDeleted: false,
                includes: q => q.Include(f => f.Product).ThenInclude(p => p!.ProductVariants)
            );
            if (!favorites.Any())
            {
                return ResponseDto<IEnumerable<FavoriteDto>>.Success(Array.Empty<FavoriteDto>(), StatusCodes.Status200OK);
            }
            var favoriteDtos = favorites.Select(f=> new FavoriteDto
            {
                Id = f.Id,
                UserId = f.UserId,
                ProductId = f.ProductId,
                CreatedDate = f.CreatedAt.UtcDateTime,
                UpdatedDate = f.UpdatedAt.UtcDateTime,
                ProductName = f.Product != null ? f.Product.Name!:f.ProductName,
                Price = f.Product != null ? (f.Product.DiscountedPrice ?? f.Product.Price) : f.Price,
                ImageUrl = f.Product != null ? f.Product.ImageUrl! : f.ImageUrl,
                IsInStock = f.Product != null && IsProductInStock(f.Product)
            }).ToList();
            return ResponseDto<IEnumerable<FavoriteDto>>.Success(favoriteDtos,StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<FavoriteDto>>.Fail($"Beklenmedik Hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public Task<ResponseDto<bool>> IsInFavoritesAsync(string userId, int productId)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> RemoveAsync(int id, string userId)
    {
        throw new NotImplementedException();
    }
    private static bool IsProductInStock(Product product)
    {
        // Varyant varsa: en az bir varyant stokta mı?
        if (product.ProductVariants?.Any(v => !v.IsDeleted && v.IsAvailable && v.Stock > 0) == true)
        {
            return true;
        }
        // Varyant yoksa: Product.Stock > 0 mı?
        return product.Stock > 0;
    }
}
