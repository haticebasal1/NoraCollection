using System;
using NoraCollection.Shared.Dtos.CategoryDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface ICategoryService
{
    Task<ResponseDto<CategoryDto>> GetAsync(int id);
    Task<ResponseDto<IEnumerable<CategoryDto>>> GetAllAsync(bool? isDeleted = false);
    Task<ResponseDto<int>> CountAsync(bool? isDeleted = false);
    Task<ResponseDto<CategoryDto>> GetBySlugAsync(string slug);
    Task<ResponseDto<CategoryDto>> AddAsync(CategoryCreateDto categoryCreateDto);
    Task<ResponseDto<NoContentDto>> UpdateAsync(CategoryUpdateDto categoryUpdateDto);
    Task<ResponseDto<NoContentDto>> HardDeleteAsync(int id);
    Task<ResponseDto<NoContentDto>> SoftDeleteAsync(int id);
}
