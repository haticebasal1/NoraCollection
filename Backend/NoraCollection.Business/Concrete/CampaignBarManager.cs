using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using NoraCollection.Business.Abstract;
using NoraCollection.Data.Abstract;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.CampaignBarDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Concrete;

public class CampaignBarManager : ICampaignBarService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<CampaignBar> _campaignBarRepository;
    private readonly IMapper _mapper;

    public CampaignBarManager(IUnitOfWork unitOfWork, IGenericRepository<CampaignBar> campaignBarRepository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _campaignBarRepository = _unitOfWork.GetRepository<CampaignBar>();
        _mapper = mapper;
    }

    public async Task<ResponseDto<CampaignBarDto>> AddAsync(CampaignBarCreateDto campaignBarCretateDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(campaignBarCretateDto.Text))
            {
                return ResponseDto<CampaignBarDto>.Fail("Metin zorunludur!", StatusCodes.Status400BadRequest);
            }
            var bar = new CampaignBar(campaignBarCretateDto.Text, campaignBarCretateDto.DisplayOrder)
            {
                Icon = campaignBarCretateDto.Icon,
                LinkUrl = campaignBarCretateDto.LinkUrl,
                IsActive = campaignBarCretateDto.IsActive,
                StartDate = campaignBarCretateDto.StartDate,
                EndDate = campaignBarCretateDto.EndDate
            };
            await _campaignBarRepository.AddAsync(bar);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<CampaignBarDto>.Fail("Kayıt sırasında hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            var dto = _mapper.Map<CampaignBarDto>(bar);
            return ResponseDto<CampaignBarDto>.Success(dto, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return ResponseDto<CampaignBarDto>.Fail($"Beklenmedik hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<CampaignBarDto>>> GetActiveBarsAsync()
    {
        try
        {
            var now = DateTime.UtcNow;

            var bars = await _campaignBarRepository.GetAllAsync(
                predicate: x => !x.IsDeleted && x.IsActive && (x.StartDate == null || x.StartDate.Value <= now) && (x.EndDate == null || x.EndDate.Value >= now),
                orderby: q => q.OrderBy(x => x.DisplayOrder),
                includeDeleted: false
            );
            var dto = _mapper.Map<IEnumerable<CampaignBarDto>>(bars);
            return ResponseDto<IEnumerable<CampaignBarDto>>.Success(dto, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<CampaignBarDto>>.Fail($"Beklenmedik hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<IEnumerable<CampaignBarDto>>> GetAllAsync(bool? includeDeleted = false, bool? isActive = null)
    {
        try
        {
            var bars = await _campaignBarRepository.GetAllAsync(
              predicate: includeDeleted == true ? null : (x => !x.IsDeleted),
              orderby: q => q.OrderBy(x => x.DisplayOrder),
              includeDeleted: includeDeleted == true
             );
            if (isActive.HasValue)
            {
                bars = bars.Where(x => x.IsActive == isActive.Value).ToList();
            }
            var dtos = _mapper.Map<IEnumerable<CampaignBarDto>>(bars);
            return ResponseDto<IEnumerable<CampaignBarDto>>.Success(dtos, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<IEnumerable<CampaignBarDto>>.Fail($"Beklenmedik hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<CampaignBarDto>> GetByIdAsync(int id)
    {
        try
        {
            var bar = await _campaignBarRepository.GetAsync(
                x => x.Id == id && !x.IsDeleted,
                includeDeleted: false
            );
            if (bar is null)
            {
                return ResponseDto<CampaignBarDto>.Fail("Kampanya bar bulunamadı!", StatusCodes.Status404NotFound);
            }
            var dto = _mapper.Map<CampaignBarDto>(bar);
            return ResponseDto<CampaignBarDto>.Success(dto, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<CampaignBarDto>.Fail($"Beklenmedik hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> SoftDeleteAsync(int id)
    {
        try
        {
            var bar = await _campaignBarRepository.GetAsync(
                x => x.Id == id,
                includeDeleted: true
            );
            if (bar is null)
            {
                return ResponseDto<NoContentDto>.Fail("Kampanya bar bulunamadı!", StatusCodes.Status404NotFound);
            }
            bar.IsDeleted = true;
            bar.DeletedAt = DateTimeOffset.UtcNow;
            bar.UpdatedAt = DateTimeOffset.UtcNow;

            _campaignBarRepository.Update(bar);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Silme sırasında hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(new NoContentDto(), StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<NoContentDto>> UpdateAsync(CampaignBarUpdateDto campaignBarUpdateDto)
    {
        try
        {
            var bar = await _campaignBarRepository.GetAsync(
                x => x.Id == campaignBarUpdateDto.Id && !x.IsDeleted,
                includeDeleted: false
            );
            if (bar is null)
            {
                return ResponseDto<NoContentDto>.Fail("Kampanya bar bulunamadı!", StatusCodes.Status404NotFound);
            }
            if (string.IsNullOrWhiteSpace(campaignBarUpdateDto.Text))
            {
                return ResponseDto<NoContentDto>.Fail("Metin zorunludur!", StatusCodes.Status400BadRequest);
            }
            bar.Text = campaignBarUpdateDto.Text;
            bar.Icon = campaignBarUpdateDto.Icon;
            bar.LinkUrl = campaignBarUpdateDto.LinkUrl;
            bar.DisplayOrder = campaignBarUpdateDto.DisplayOrder;
            bar.IsActive = campaignBarUpdateDto.IsActive;
            bar.StartDate = campaignBarUpdateDto.StartDate;
            bar.EndDate = campaignBarUpdateDto.EndDate;
            bar.UpdatedAt = DateTimeOffset.UtcNow;

            _campaignBarRepository.Update(bar);
            var result = await _unitOfWork.SaveAsync();
            if (result < 1)
            {
                return ResponseDto<NoContentDto>.Fail("Güncelleme sırasında hata oluştu!", StatusCodes.Status500InternalServerError);
            }
            return ResponseDto<NoContentDto>.Success(new NoContentDto(), StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail($"Beklenmedik hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }
}
