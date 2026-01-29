using System;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using NoraCollection.Business.Abstract;
using NoraCollection.Data.Abstract;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.HeroBannerDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Concrete;

public class HeroBannerManager : IHeroBannerService
{
    public readonly IUnitOfWork _unitOfWork;
    public readonly IGenericRepository<HeroBanner> _herobannerRepository;
    public readonly IMapper _mapper;
    public readonly IHttpContextAccessor _httpContextAccessor;
    public readonly IImageService _imageService;

    public HeroBannerManager(IUnitOfWork unitOfWork, IGenericRepository<HeroBanner> herobannerRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IImageService imageService)
    {
        _unitOfWork = unitOfWork;
        _herobannerRepository = _unitOfWork.GetRepository<HeroBanner>();
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _imageService = imageService;
    }

    public async Task<ResponseDto<HeroBannerDto>> AddAsync(HeroBannerCreateDto heroBannerCreateDto)
    {
        try
        {
            // Zorunlu alan kontrolleri
            if (string.IsNullOrWhiteSpace(heroBannerCreateDto.Title))
            {
                return ResponseDto<HeroBannerDto>.Fail("Başlık zorunludur!", StatusCodes.Status400BadRequest);
            }
            if (heroBannerCreateDto.Image is null)
            {
                return ResponseDto<HeroBannerDto>.Fail("Banner görseli zorunludur!", StatusCodes.Status400BadRequest);
            }
            // Ana görseli yükle (resize + wwwroot/images/hero-banners altına kaydet)
            var imageUploadResult = await _imageService.ResizeAndUploadAsync(heroBannerCreateDto.Image, "hero-banners");
            if (!imageUploadResult.IsSuccessful)
            {
                return ResponseDto<HeroBannerDto>.Fail(imageUploadResult.Errors, imageUploadResult.StatusCode);
            }
            // Tarayıcıda kullanılacak tam URL (https://site.com/images/hero-banners/xxx.jpg)
            var baseUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var newImageUrl = $"{baseUrl}/{imageUploadResult.Data.TrimStart('/')}";
            // Mobil görsel opsiyonel: varsa yükle; hata alırsan ana görseli de silip işlemi iptal et
            string? mobileImageUrl = null;
            if (heroBannerCreateDto.MobileImage is not null)
            {
                var mobileUploadResult = await _imageService.ResizeAndUploadAsync(heroBannerCreateDto.MobileImage, "hero-banners");
                if (!mobileUploadResult.IsSuccessful)
                {
                    _imageService.DeleteImage(newImageUrl);
                    return ResponseDto<HeroBannerDto>.Fail(mobileUploadResult.Errors, mobileUploadResult.StatusCode);
                }
                mobileImageUrl = $"{baseUrl}/{mobileUploadResult.Data.TrimStart('/')}";
            }
            // Dto → Entity map (Title, Subtitle, ButtonText, DisplayOrder, IsActive, StartDate, EndDate vb.)
            var heroBanner = _mapper.Map<HeroBanner>(heroBannerCreateDto);
            heroBanner.ImageUrl = newImageUrl;
            heroBanner.MobileImageUrl = mobileImageUrl;
            // Kaydet
            await _herobannerRepository.AddAsync(heroBanner);
            var result = await _unitOfWork.SaveAsync();

            if (result < 1)
            {
                _imageService.DeleteImage(newImageUrl);
                if (mobileImageUrl is not null)
                    _imageService.DeleteImage(mobileImageUrl);
                return ResponseDto<HeroBannerDto>.Fail("Banner kaydedilirken bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }

            var heroBannerDto = _mapper.Map<HeroBannerDto>(heroBanner);
            return ResponseDto<HeroBannerDto>.Success(heroBannerDto, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return ResponseDto<HeroBannerDto>.Fail($"Banner eklenirken bir hata oluştu:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<HeroBannerDto>>> GetActiveBannersAsync()
    {
        try
        {
            // "Bugün" için referans tarih (StartDate/EndDate karşılaştırmasında kullanılacak)
            var today = DateTimeOffset.UtcNow.Date;

            // Sadece şu an yayında sayılan banner'lar:
            // - Silinmemiş, aktif ve tarih aralığına uyan kayıtlar
            Expression<Func<HeroBanner, bool>> predicate = x =>
            !x.IsDeleted
           && x.IsActive
           && (x.StartDate == null || x.StartDate.Value.Date <= today) // Başlangıç bugün veya geçmişte
           && (x.EndDate == null || x.EndDate.Value.Date >= today);  // Bitiş bugün veya geçmişte
            // Önce DisplayOrder'a göre, aynı sıra numarasında Title'a göre sırala
            Func<IQueryable<HeroBanner>, IOrderedQueryable<HeroBanner>> orderby = q =>
            q.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Title);
            // Filtreli ve sıralı liste çek (silinmişleri dahil etme)
            var banners = await _herobannerRepository.GetAllAsync(
             predicate: predicate,
             orderby: orderby,
             includeDeleted: false
            );
            //Entity-dto dönüşümü
            var heroBannerDto = _mapper.Map<IEnumerable<HeroBannerDto>>(banners);
            return ResponseDto<IEnumerable<HeroBannerDto>>.Success(heroBannerDto, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<HeroBannerDto>>.Fail(
            $"Aktif banner listesi getirilirken hata oluştu: {ex.Message}",
            StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<HeroBannerDto>>> GetAllAsync(bool? isdeleted = false, bool? isActive = null)
    {
        try
        {
            Expression<Func<HeroBanner, bool>> predicate;
            bool includeDeleted;
            if (isdeleted == null)
            {
                includeDeleted = true;
                predicate = x => true;
            }
            else if (isdeleted == true)
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
            Func<IQueryable<HeroBanner>, IOrderedQueryable<HeroBanner>> orderBy = q => q.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Title);
            var heroBanners = await _herobannerRepository.GetAllAsync(
                predicate: predicate,
                orderby: orderBy,
                includeDeleted: includeDeleted
            );
            var heroBannerDto = _mapper.Map<IEnumerable<HeroBannerDto>>(heroBanners);
            return ResponseDto<IEnumerable<HeroBannerDto>>.Success(heroBannerDto, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<HeroBannerDto>>.Fail(
            $"Banner listesi getirilirken hata oluştu: {ex.Message}",
            StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<HeroBannerDto>> GetByIdAsync(int id)
    {
        try
        {
            var heroBanner = await _herobannerRepository.GetAsync(
                x => x.Id == id && !x.IsDeleted,
                includeDeleted: false
            );
            if (heroBanner is null)
            {
                return ResponseDto<HeroBannerDto>.Fail($"{id} id'li banner bulunamdı!", StatusCodes.Status404NotFound);
            }
            var heroBannerDto = _mapper.Map<HeroBannerDto>(heroBanner);
            return ResponseDto<HeroBannerDto>.Success(heroBannerDto, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<HeroBannerDto>.Fail(
            $"Banner getirilirken hata oluştu: {ex.Message}",
            StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> HardDeleteAsync(int id)
    {
        try
        {
            var heroBanner = await _herobannerRepository.GetAsync(
                predicate: x => x.Id == id,
                includeDeleted: true
            );
            if (heroBanner is null)
            {
                return ResponseDto<NoContentDto>.Fail($"{id} id'li banner bulunamadı!", StatusCodes.Status404NotFound);
            }
            _herobannerRepository.Delete(heroBanner);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Banner silinirken bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            if (!string.IsNullOrWhiteSpace(heroBanner.ImageUrl))
            {
                _imageService.DeleteImage(heroBanner.ImageUrl);
            }
            if (!string.IsNullOrWhiteSpace(heroBanner.MobileImageUrl))
            {
                _imageService.DeleteImage(heroBanner.MobileImageUrl);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail(
            $"Banner silinirken hata oluştu: {ex.Message}",
            StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> SoftDeleteAsync(int id)
    {
        try
        {
            var heroBanner = await _herobannerRepository.GetAsync(
               predicate: x => x.Id == id && !x.IsDeleted,
               includeDeleted: true
            );
            if (heroBanner is null)
            {
                return ResponseDto<NoContentDto>.Fail($"{id} id'li banner bulunamadı!", StatusCodes.Status404NotFound);
            }
            heroBanner.IsDeleted = true;
            heroBanner.DeletedAt = DateTimeOffset.UtcNow;
            _herobannerRepository.Update(heroBanner);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Banner silinirken bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail(
            $"Banner silinirken hata oluştu: {ex.Message}",
            StatusCodes.Status500InternalServerError);
        }
    }

public async Task<ResponseDto<NoContentDto>> UpdateAsync(HeroBannerUpdateDto heroBannerUpdateDto)
{
    try
    {
        // Kaydı bul (silinmemiş)
        var heroBanner = await _herobannerRepository.GetAsync(
            predicate: x => x.Id == heroBannerUpdateDto.Id && !x.IsDeleted,
            includeDeleted: false
        );
        if (heroBanner is null)
        {
            return ResponseDto<NoContentDto>.Fail($"{heroBannerUpdateDto.Id} id'li banner bulunamadı!", StatusCodes.Status404NotFound);
        }

        // Title duplicate kontrolü
        var isTitleExists = await _herobannerRepository.ExistsAsync(
            x => x.Title!.ToLower() == heroBannerUpdateDto.Title.ToLower() 
            && x.Id != heroBannerUpdateDto.Id
        );
        if (isTitleExists)
        {
            return ResponseDto<NoContentDto>.Fail("Bu başlıkta zaten bir banner mevcut!", StatusCodes.Status400BadRequest);
        }

        // Eski görsel URL'lerini sakla
        var oldImageUrl = heroBanner.ImageUrl;
        var oldMobileImageUrl = heroBanner.MobileImageUrl;

        // Ana görsel yükleme (varsa)
        string? newImageUrl = null;
        if (heroBannerUpdateDto.Image is not null)
        {
            var imageUploadResult = await _imageService.ResizeAndUploadAsync(heroBannerUpdateDto.Image, "hero-banners");
            if (!imageUploadResult.IsSuccessful)
            {
                return ResponseDto<NoContentDto>.Fail(imageUploadResult.Errors, imageUploadResult.StatusCode);
            }
            var baseUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            newImageUrl = $"{baseUrl}/{imageUploadResult.Data!.TrimStart('/')}";
        }

        // Mobil görsel yükleme (varsa)
        string? newMobileImageUrl = null;
        if (heroBannerUpdateDto.MobileImage is not null)
        {
            var mobileUploadResult = await _imageService.ResizeAndUploadAsync(heroBannerUpdateDto.MobileImage, "hero-banners");
            if (!mobileUploadResult.IsSuccessful)
            {
                // Mobil görsel hatası: yeni ana görsel yüklenmişse onu da sil (rollback)
                if (newImageUrl is not null)
                    _imageService.DeleteImage(newImageUrl);
                return ResponseDto<NoContentDto>.Fail(mobileUploadResult.Errors, mobileUploadResult.StatusCode);
            }
            var baseUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            newMobileImageUrl = $"{baseUrl}/{mobileUploadResult.Data!.TrimStart('/')}";
        }

        // Dto'dan entity'ye map (Image/MobileImage ignore edilir)
        _mapper.Map(heroBannerUpdateDto, heroBanner);

        // Yeni görsel URL'lerini set et (varsa)
        if (newImageUrl is not null)
        {
            heroBanner.ImageUrl = newImageUrl;
        }
        if (newMobileImageUrl is not null)
        {
            heroBanner.MobileImageUrl = newMobileImageUrl;
        }

        // UpdatedAt güncelle
        heroBanner.UpdatedAt = DateTimeOffset.UtcNow;

        // Kaydet
        _herobannerRepository.Update(heroBanner);
        var result = await _unitOfWork.SaveAsync();

        // Save başarısızsa yeni yüklenen görselleri sil (rollback)
        if (result < 1)
        {
            if (newImageUrl is not null)
                _imageService.DeleteImage(newImageUrl);
            if (newMobileImageUrl is not null)
                _imageService.DeleteImage(newMobileImageUrl);
            return ResponseDto<NoContentDto>.Fail("Beklenmedik hata oluştu!", StatusCodes.Status500InternalServerError);
        }

        // Save başarılıysa eski görselleri sil (yeni görsel varsa)
        if (newImageUrl is not null && !string.IsNullOrWhiteSpace(oldImageUrl))
        {
            _imageService.DeleteImage(oldImageUrl);
        }
        if (newMobileImageUrl is not null && !string.IsNullOrWhiteSpace(oldMobileImageUrl))
        {
            _imageService.DeleteImage(oldMobileImageUrl);
        }

        return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
    }
    catch (Exception ex)
    {
        return ResponseDto<NoContentDto>.Fail(
            $"Banner güncellenirken hata oluştu: {ex.Message}",
            StatusCodes.Status500InternalServerError);
    }
}

    public Task<ResponseDto<NoContentDto>> UpdateOrderAsync(int id, int newOrder)
    {
        throw new NotImplementedException();
    }
    private Expression<Func<HeroBanner, bool>> CombinePredicates(
       Expression<Func<HeroBanner, bool>> first,
       Expression<Func<HeroBanner, bool>> second
   )
    {
        // Ortak bir parametre oluştur (her iki expression'da da "x" kullanılıyor)
        var parameter = Expression.Parameter(typeof(HeroBanner), "x");
        // İlk expression'daki parametreyi yeni parametreyle değiştir
        var leftVisitor = new ReplaceExpressionVisitor(first.Parameters[0], parameter);
        var left = leftVisitor.Visit(first.Body);
        // İkinci expression'daki parametreyi yeni parametreyle değiştir
        var rightVisitor = new ReplaceExpressionVisitor(second.Parameters[0], parameter);
        var right = rightVisitor.Visit(second.Body);
        // İki expression'ı AND operatörü ile birleştir
        return Expression.Lambda<Func<HeroBanner, bool>>(Expression.AndAlso(left!, right!), parameter);
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
