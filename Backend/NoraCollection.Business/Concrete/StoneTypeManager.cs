using System;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using NoraCollection.Business.Abstract;
using NoraCollection.Data.Abstract;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.ResponseDtos;
using NoraCollection.Shared.Dtos.StoneTypeDtos;

namespace NoraCollection.Business.Concrete;

public class StoneTypeManager : IStoneTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGenericRepository<StoneType> _stoneTypeRepository;
    private readonly IGenericRepository<Product> _productRepository;

    public StoneTypeManager(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService, IHttpContextAccessor httpContextAccessor, IGenericRepository<StoneType> stoneTypeRepository, IGenericRepository<Product> productRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageService = imageService;
        _httpContextAccessor = httpContextAccessor;
        _stoneTypeRepository = unitOfWork.GetRepository<StoneType>();
        _productRepository = unitOfWork.GetRepository<Product>();
    }

    public async Task<ResponseDto<StoneTypeDto>> AddAsync(StoneTypeCreateDto stoneTypeCreateDto)
    {
        try
        {
            //İsim benzerlik kontrol
            var isExists = await _stoneTypeRepository.ExistsAsync(
                x => x.Name.ToLower() == stoneTypeCreateDto.Name.ToLower() && !x.IsDeleted
            );
            if (isExists)
            {
                return ResponseDto<StoneTypeDto>.Fail("Bu isimde bir taş tipi zaten mevcut!", StatusCodes.Status400BadRequest);
            }
            // Resim kontrollü
            string? newImageUrl = null;
            if (stoneTypeCreateDto.Image is not null)
            {
                var imageUploadResult = await _imageService.ResizeAndUploadAsync(stoneTypeCreateDto.Image, "stone-types");
                if (!imageUploadResult.IsSuccessful)
                {
                    return ResponseDto<StoneTypeDto>.Fail(
                        imageUploadResult.Errors,
                        imageUploadResult.StatusCode
                    );
                }
                var baseUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
                newImageUrl = $"{baseUrl}/{imageUploadResult.Data.TrimStart('/')}";
            }
            //Mapping
            var stoneType = _mapper.Map<StoneType>(stoneTypeCreateDto);
            // ImageUrl'i manuel set et (Mapping'de ignore edilmişti)
            if (newImageUrl is not null)
            {
                stoneType.ImageUrl = newImageUrl;
            }
            //Veri tabanına kaydet
            await _stoneTypeRepository.AddAsync(stoneType);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                if (newImageUrl is not null)
                {
                    _imageService.DeleteImage(newImageUrl);
                }
                return ResponseDto<StoneTypeDto>.Fail("Taş tipi kaydedilirken beklenmedik hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            var stoneTypeDto = _mapper.Map<StoneTypeDto>(stoneType);
            return ResponseDto<StoneTypeDto>.Success(stoneTypeDto, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return ResponseDto<StoneTypeDto>.Fail($"Taş tipi eklenirken bir hata oluştu!:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<int>> CountAsync(bool? isDeleted = false, bool? isActive = null)
    {
        try
        {
            Expression<Func<StoneType, bool>> predicate = x => true;
            bool includeDeleted = false;
            if (isDeleted.HasValue)
            {
                var deletedValue = isDeleted.Value;
                includeDeleted = deletedValue;
                predicate = CombinePredicates(predicate, x => x.IsDeleted == deletedValue);
            }
            else
            {
                includeDeleted = true;
            }
            if (isActive.HasValue)
            {
                var activeValue = isActive.Value;
                predicate = CombinePredicates(predicate, x => x.IsActive == activeValue);
            }
            var count = await _stoneTypeRepository.CountAsync(
                predicate: predicate,
                includeDeleted: includeDeleted
            );
            return ResponseDto<int>.Success(count, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<int>.Fail($"Taş tipi sayısı hesaplanırken bir hata oluştu!:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<StoneTypeDto>>> GetAllAsync(bool? isDeleted = false, bool? isActive = null)
    {
        try
        {
            //Predicate oluşturma
            Expression<Func<StoneType, bool>> predicate;
            bool includeDeleted;
            if (isDeleted == null)
            {
                includeDeleted = true;
                predicate = x => true;
            }
            else if (isDeleted == true)
            {
                includeDeleted = true;
                predicate = x => x.IsDeleted;
            }
            else
            {
                includeDeleted = false;
                predicate = x => !x.IsDeleted;
            }
            //isActive filtresi
            if (isActive.HasValue)
            {
                var activeValue = isActive.Value;
                predicate = CombinePredicates(predicate, x => x.IsActive == activeValue);
            }
            //Sıralama(displayorder sonra name e göre)
            Func<IQueryable<StoneType>, IOrderedQueryable<StoneType>> orderByFunc
            = query => query.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name);
            //Repositoryden veri çekme
            var stoneTypes = await _stoneTypeRepository.GetAllAsync(
                predicate: predicate,
                orderby: orderByFunc,
                includeDeleted: includeDeleted
            );
            var stoneTypeDtos = _mapper.Map<IEnumerable<StoneTypeDto>>(stoneTypes);
            return ResponseDto<IEnumerable<StoneTypeDto>>.Success(stoneTypeDtos, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<StoneTypeDto>>.Fail($"Taş tipi getirilirken bir hata oluştu!:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<StoneTypeDto>> GetAsync(int id)
    {
        try
        {
            var stoneType = await _stoneTypeRepository.GetAsync(
             x => x.Id == id && !x.IsDeleted,
             includeDeleted: false
            );

            if (stoneType is null)
            {
                return ResponseDto<StoneTypeDto>.Fail($"{id} id'li taş tipi bulunamadı!", StatusCodes.Status404NotFound);
            }

            var stoneTypeDto = _mapper.Map<StoneTypeDto>(stoneType);
            return ResponseDto<StoneTypeDto>.Success(stoneTypeDto, StatusCodes.Status200OK);
        }

        catch (Exception ex)
        {
            return ResponseDto<StoneTypeDto>.Fail($"Taş tipi getirilirken bir hata oluştu!:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> HardDeleteAsync(int id)
    {
        try
        {
            var stoneType = await _stoneTypeRepository.GetAsync(
                predicate: x => x.Id == id && !x.IsDeleted,
                includeDeleted: true
            );
            if (stoneType is null)
            {
                return ResponseDto<NoContentDto>.Fail($"{id} id'li taş tipi bulunamadı!", StatusCodes.Status404NotFound);
            }
            var hasProducts = await _productRepository.ExistsAsync(
                x => x.StoneTypeId == id && !x.IsDeleted
            );
            if (hasProducts)
            {
                return ResponseDto<NoContentDto>.Fail("Bu taş tipine bağlı aktif ürünler olduğu için silinemez!", StatusCodes.Status400BadRequest);
            }
            _stoneTypeRepository.Delete(stoneType);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Taş tipi silinirken bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            if (!string.IsNullOrWhiteSpace(stoneType.ImageUrl))
            {
                _imageService.DeleteImage(stoneType.ImageUrl);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Taş tipi silinirken bir hata oluştu!:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> SoftDeleteAsync(int id)
    {
        try
        {
            var stoneType = await _stoneTypeRepository.GetAsync(
              predicate: x => x.Id == id && !x.IsDeleted,
              includeDeleted: true
            );
            if (stoneType is null)
            {
                return ResponseDto<NoContentDto>.Fail($"{id} id'li taş tipi bulunamadı!", StatusCodes.Status404NotFound);
            }
            var hasProducts = await _productRepository.ExistsAsync(
                x => x.StoneTypeId == id && !x.IsDeleted
            );
            if (hasProducts)
            {
                return ResponseDto<NoContentDto>.Fail("Bu taş tipine bağlı aktif ürünler olduğu için silinemez!", StatusCodes.Status400BadRequest);
            }
            stoneType.IsDeleted = true;
            stoneType.DeletedAt = DateTimeOffset.UtcNow;
            _stoneTypeRepository.Update(stoneType);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Taş tipi silinirken bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Taş tipi silinirken bir hata oluştu!:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> ToggleIsActiveAsync(int id)
    {
        try
        {
            var stoneType = await _stoneTypeRepository.GetAsync(
                x => x.Id == id && !x.IsDeleted,
                includeDeleted: false
            );
            if (stoneType is null)
            {
                return ResponseDto<NoContentDto>.Fail($"{id} id'li taş tipi bulunamadı!", StatusCodes.Status404NotFound);
            }
            stoneType.IsActive = !stoneType.IsActive;
            stoneType.UpdatedAt = DateTimeOffset.UtcNow;
            _stoneTypeRepository.Update(stoneType);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Taş tipi güncellenirken bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Taş tipi güncellenirken bir hata oluştu!:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> UpdateAsync(StoneTypeUpdateDto stoneTypeUpdateDto)
    {
        try
        {
            var stoneType = await _stoneTypeRepository.GetAsync(
               x => x.Id == stoneTypeUpdateDto.Id && !x.IsDeleted,
               includeDeleted: false
            );
            if (stoneType is null)
            {
                return ResponseDto<NoContentDto>.Fail($"{stoneTypeUpdateDto.Id} id'li taş tipi bulunamadı!", StatusCodes.Status404NotFound);
            }
            var isNameExists = await _stoneTypeRepository.ExistsAsync(x => x.Name!.ToLower() == stoneTypeUpdateDto.Name.ToLower() && x.Id != stoneTypeUpdateDto.Id);
            if (isNameExists)
            {
                return ResponseDto<NoContentDto>.Fail("Bu isimde zaten bir taş tipi mevcut!", StatusCodes.Status400BadRequest);
            }
            var oldImageUrl = stoneType.ImageUrl;
            string? newImageUrl = null;
            if (stoneTypeUpdateDto.Image is not null)
            {
                var imageUploadResult = await _imageService.ResizeAndUploadAsync(stoneTypeUpdateDto.Image, "stone-types");
                if (!imageUploadResult.IsSuccessful)
                {
                    return ResponseDto<NoContentDto>.Fail(imageUploadResult.Errors, imageUploadResult.StatusCode);
                }
                var baseUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
                newImageUrl = $"{baseUrl}/{imageUploadResult.Data!.TrimStart('/')}";
            }
            _mapper.Map(stoneTypeUpdateDto, stoneType);
            if (newImageUrl is not null)
            {
                stoneType.ImageUrl = newImageUrl;
            }
            stoneType.UpdatedAt = DateTimeOffset.UtcNow;
            _stoneTypeRepository.Update(stoneType);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                if (newImageUrl is not null) _imageService.DeleteImage(newImageUrl);
                return ResponseDto<NoContentDto>.Fail("Beklenmedik hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            if (newImageUrl is not null && !string.IsNullOrWhiteSpace(oldImageUrl))
            {
                _imageService.DeleteImage(oldImageUrl);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Taş tipi güncellenirken bir hata oluştu!:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }
    private Expression<Func<StoneType, bool>> CombinePredicates(
       Expression<Func<StoneType, bool>> first,
       Expression<Func<StoneType, bool>> second
   )
    {
        // Ortak bir parametre oluştur (her iki expression'da da "x" kullanılıyor)
        var parameter = Expression.Parameter(typeof(StoneType), "x");
        // İlk expression'daki parametreyi yeni parametreyle değiştir
        var leftVisitor = new ReplaceExpressionVisitor(first.Parameters[0], parameter);
        var left = leftVisitor.Visit(first.Body);
        // İkinci expression'daki parametreyi yeni parametreyle değiştir
        var rightVisitor = new ReplaceExpressionVisitor(second.Parameters[0], parameter);
        var right = rightVisitor.Visit(second.Body);
        // İki expression'ı AND operatörü ile birleştir
        return Expression.Lambda<Func<StoneType, bool>>(Expression.AndAlso(left!, right!), parameter);
    }
    // Expression'lardaki parametre çakışmasını önlemek için yardımcı class
    // Bu class, bir expression'daki parametreyi başka bir parametreyle değiştirir
    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _from;
        private readonly Expression _to;

        public ReplaceExpressionVisitor(Expression from, Expression to)
        {
            _from = from;
            _to = to;
        }
        public override Expression? Visit(Expression? node)
        {
            // Eğer ziyaret edilen node, değiştirilmesi gereken parametre ise, yeni parametreyi döndür
            // Değilse, normal ziyaret işlemini devam ettir
            return node == _from ? _to : base.Visit(node);
        }
    }
}
