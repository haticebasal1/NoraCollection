using System;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using NoraCollection.Business.Abstract;
using NoraCollection.Data.Abstract;
using NoraCollection.Entities.Concrete;
using NoraCollection.Shared.Dtos.CategoryDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Concrete;

public class CategoryManager : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IGenericRepository<ProductCategory> _productCategoryRepository;
    private readonly IMapper _mapper;
    private readonly IImageService _imageManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CategoryManager(IUnitOfWork unitOfWork, IGenericRepository<Category> categoryRepository, IGenericRepository<ProductCategory> productCategoryRepository, IMapper mapper, IImageService imageManager, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _categoryRepository = _unitOfWork.GetRepository<Category>();
        _productCategoryRepository = _unitOfWork.GetRepository<ProductCategory>();
        _mapper = mapper;
        _imageManager = imageManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ResponseDto<CategoryDto>> AddAsync(CategoryCreateDto categoryCreateDto)
    {
        try
        {
            var isExists = await _categoryRepository.ExistsAsync(x => x.Name!.ToLower() == categoryCreateDto.Name.ToLower());
            if (isExists)
            {
                return ResponseDto<CategoryDto>.Fail("Bu isimde bir kategori mevcut!", StatusCodes.Status400BadRequest);
            }
            if (categoryCreateDto.Image is null)
            {
                return ResponseDto<CategoryDto>.Fail("Resim zorunludur!", StatusCodes.Status400BadRequest);
            }
            var imageUploadResult = await _imageManager.UploadAsync(categoryCreateDto.Image, "categories");
            if (!imageUploadResult.IsSuccessful)
            {
                return ResponseDto<CategoryDto>.Fail(imageUploadResult.Errors, imageUploadResult.StatusCode);
            }
            var category = _mapper.Map<Category>(categoryCreateDto);
            category.Slug = GenerateSlug(categoryCreateDto.Name);
            var baseUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            category.ImageUrl = $"{baseUrl}/{imageUploadResult.Data.TrimStart('/')}";

            await _categoryRepository.AddAsync(category);
            var result = await _unitOfWork.SaveAsync();

            if (result < 1)
            {
                return ResponseDto<CategoryDto>.Fail("Beklenmedik bir hata oluştu!", StatusCodes.Status500InternalServerError);
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return ResponseDto<CategoryDto>.Success(categoryDto, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return ResponseDto<CategoryDto>.Fail($"Hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<int>> CountAsync(bool? isDeleted = false)
    {
        try
        {
            int count;
            if (isDeleted == null)
            {
                count = await _categoryRepository.CountAsync(includeDeleted:true);
            }
            else if (isDeleted == true)
            {
                count = await _categoryRepository.CountAsync(
                    predicate: x=>x.IsDeleted == true,
                    includeDeleted : true
                );
            }
            else
            {
                count = await _categoryRepository.CountAsync(includeDeleted: false);
            }
            return ResponseDto<int>.Success(count, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<int>.Fail($"Hata: {ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public Task<ResponseDto<IEnumerable<CategoryDto>>> GetAllAsync(bool? isDeleted = false)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<CategoryDto>> GetAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<CategoryDto>> GetBySlugAsync(string slug)
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

    public Task<ResponseDto<NoContentDto>> UpdateAsync(CategoryUpdateDto categoryUpdateDto)
    {
        throw new NotImplementedException();
    }
    private string GenerateSlug(string name)
    {
        var slug = name.ToLowerInvariant()
        .Replace(" ", "-")
        .Replace("ı", "i")
        .Replace("ğ", "g")
        .Replace("ü", "u")
        .Replace("ş", "s")
        .Replace("ö", "o")
        .Replace("ç", "c");
        slug = Regex.Replace(slug, @"[^a-z0-9\-]", ""); // Özel karakterleri kaldır
        slug = Regex.Replace(slug, @"-+", "-");// Çoklu tireleri tek tireye dönüştür
        slug = slug.Trim('-');// Baş ve sondaki tireleri kaldır
        return slug;
    }
}
