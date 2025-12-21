using System;
using NoraCollection.Shared.Dtos.BlogPostDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IBlogPostService
{
  Task<ResponseDto<BlogPostDto>> GetByIdAsync(int id); // Tekil Getirme
  Task<ResponseDto<BlogPostDto>> GetBySlugAsync(string slug);
  Task<ResponseDto<IEnumerable<BlogPostDto>>> GetAllAsync(int? categoryId = null, bool? isPublished = null);  // Çoklu Getirme
  Task<ResponseDto<IEnumerable<BlogPostDto>>> GetRecentAsync(int top = 5);
  Task<ResponseDto<BlogPostDto>> AddAsync(BlogPostCreateDto blogPostCreateDto);//Crud
  Task<ResponseDto<NoContentDto>> UpdateAsync(BlogPostUpdateDto blogPostUpdateDto);
  Task<ResponseDto<NoContentDto>> DeleteAsync(int id);
  Task<ResponseDto<NoContentDto>> PublishAsync(int id); // Durum
  Task<ResponseDto<NoContentDto>> UnPublishAsync(int id);
  Task<ResponseDto<NoContentDto>> IncrementViewCountAsync(int id); // İstatistik
  Task<ResponseDto<int>> CountAsync(int? categoryId = null);
}
