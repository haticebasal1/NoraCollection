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

    public Task<ResponseDto<IEnumerable<HeroBannerDto>>> GetAllAsync(bool? isdeleted = false, bool? isActive = null)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<HeroBannerDto>> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> HardDeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> SoftDeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> UpdateAsync(HeroBannerUpdateDto heroBannerUpdateDto)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<NoContentDto>> UpdateOrderAsync(int id, int newOrder)
    {
        throw new NotImplementedException();
    }
}
