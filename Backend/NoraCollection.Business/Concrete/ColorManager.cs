using System;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using NoraCollection.Business.Abstract;
using NoraCollection.Data.Abstract;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.ColorDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Concrete;

public class ColorManager : IColorService
{
    public readonly IUnitOfWork _unitOfWork;
    public readonly IMapper _mapper;
    public readonly IGenericRepository<ProductImage> _productImageRepository;
    public readonly IGenericRepository<ProductVariant> _productVariantRepository;
    public readonly IGenericRepository<Product> _productRepository;
    public readonly IGenericRepository<Color> _colorRepository;

    public ColorManager(IUnitOfWork unitOfWork, IMapper mapper, IGenericRepository<ProductImage> productImageRepository, IGenericRepository<ProductVariant> productVariantRepository, IGenericRepository<Product> productRepository, IGenericRepository<Color> colorRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _productImageRepository = _unitOfWork.GetRepository<ProductImage>();
        _productVariantRepository = _unitOfWork.GetRepository<ProductVariant>();
        _productRepository = _unitOfWork.GetRepository<Product>();
        _colorRepository = _unitOfWork.GetRepository<Color>();
    }

    public async Task<ResponseDto<ColorDto>> AddAsync(ColorCreateDto colorCreateDto)
    {
        try
        {
            // 1. İsim ve HexCode için normalizasyon (Karşılaştırma garantisi için)
            var normalizadeName = colorCreateDto.Name.Trim().ToLower();
            // Mapping'deki mantığın aynısını kontrol için burada da uyguluyoruz
            var normalizadeHex = colorCreateDto.HexCode.Trim().ToUpper().StartsWith("#")
            ? colorCreateDto.HexCode.Trim().ToUpper()
            : $"{colorCreateDto.HexCode.Trim().ToUpper()}";
            var isExists = await _colorRepository.ExistsAsync(
               x => !x.IsDeleted &&
                 (x.Name.ToLower() == normalizadeName || x.HexCode == normalizadeHex)
            );
            if (isExists)
            {
                return ResponseDto<ColorDto>.Fail("Bu renk isminden bir renk ismi mevcut!", StatusCodes.Status400BadRequest);
            }
            if (string.IsNullOrWhiteSpace(colorCreateDto.HexCode))
            {
                return ResponseDto<ColorDto>.Fail("Hex kod zorunludur!", StatusCodes.Status400BadRequest);
            }
            var color = _mapper.Map<Color>(colorCreateDto);
            await _colorRepository.AddAsync(color);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<ColorDto>.Fail("Renk kaydedilirken bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            var colorDto = _mapper.Map<ColorDto>(color);
            return ResponseDto<ColorDto>.Success(colorDto, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return ResponseDto<ColorDto>.Fail($"Renk eklenirken bir hata oluştu!:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<int>> CountAsync(bool? isDeleted = false, bool? isActive = null)
    {
        try
        {
            Expression<Func<Color, bool>> predicate = x => true;
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
                predicate = CombinePredicates(predicate, x => isActive == activeValue);
            }
            var count = await _colorRepository.CountAsync(
                predicate: predicate,
                includeDeleted: includeDeleted
            );
            return ResponseDto<int>.Success(count, StatusCodes.Status200OK);

        }
        catch (Exception ex)
        {
            return ResponseDto<int>.Fail($"Renk sayısı hesaplanırken bir hata oluştu!:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<ColorDto>>> GetAllAsync(bool? isDeleted = false, bool? isActive = null)
    {
        try
        {
            Expression<Func<Color, bool>> predicate;
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
            if (isActive.HasValue)
            {
                var activeValue = isActive.Value;
                predicate = CombinePredicates(predicate, x => x.IsActive == activeValue);
            }
            Func<IQueryable<Color>,IOrderedQueryable<Color>> orderByFunc = query =>query.OrderBy(x=>x.DisplayOrder).ThenBy(x=>x.Name);
            var colors = await _colorRepository.GetAllAsync(
                predicate:predicate,
                orderby:orderByFunc,
                includeDeleted:includeDeleted
            );
            var colorDtos = _mapper.Map<IEnumerable<ColorDto>>(colors);
            return ResponseDto<IEnumerable<ColorDto>>.Success(colorDtos,StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<ColorDto>>.Fail($"Renk getirilirken bir hata oluştu!:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<ColorDto>> GetAsync(int id)
    {
        try
        {
            var color = await _colorRepository.GetAsync(
                x=>x.Id == id && !x.IsDeleted,
                includeDeleted:false
            );
            if (color is null)
            {
                return ResponseDto<ColorDto>.Fail($"{id} id'li renk bulunamadı!",StatusCodes.Status404NotFound);
            }
            var colorDto = _mapper.Map<ColorDto>(color);
            return ResponseDto<ColorDto>.Success(colorDto,StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<ColorDto>.Fail($"Renk getirilirken bir hata oluştu!:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public Task<ResponseDto<NoContentDto>> HardDeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> SoftDeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> ToggleIsActiveAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> UpdateAsync(ColorUpdateDto colorUpdateDto)
    {
        throw new NotImplementedException();
    }
    private Expression<Func<Color, bool>> CombinePredicates(
       Expression<Func<Color, bool>> first,
       Expression<Func<Color, bool>> second
    )
    {
        // Ortak bir parametre oluştur (her iki expression'da da "x" kullanılıyor)
        var parameter = Expression.Parameter(typeof(Color), "x");
        // İlk expression'daki parametreyi yeni parametreyle değiştir
        var leftVisitor = new ReplaceExpressionVisitor(first.Parameters[0], parameter);
        var left = leftVisitor.Visit(first.Body);
        // İkinci expression'daki parametreyi yeni parametreyle değiştir
        var rightVisitor = new ReplaceExpressionVisitor(second.Parameters[0], parameter);
        var right = rightVisitor.Visit(second.Body);
        // İki expression'ı AND operatörü ile birleştir
        return Expression.Lambda<Func<Color, bool>>(Expression.AndAlso(left!, right!), parameter);
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
