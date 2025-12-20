using System;
using NoraCollection.Shared.Dtos.BlogCategoryDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IBlogCategoryService
{
    Task<ResponseDto<BlogCategoryDto>> GetByIdAsync(int id);
    Task<ResponseDto<BlogCategoryDto>> GetBySlugAsync(string slug);
    Task<ResponseDto<IEnumerable<BlogCategoryDto>>> GetAllAsync();
    Task<ResponseDto<BlogCategoryDto>> AddAsync(BlogCategoryCreateDto blogCategoryCreateDto);
    Task<ResponseDto<NoContentDto>> UpdateAsync(BlogCategoryUpdateDto blogCategoryUpdateDto);
    Task<ResponseDto<NoContentDto>> DeleteAsync(int id);
    Task<ResponseDto<int>> CountAsync();
}
