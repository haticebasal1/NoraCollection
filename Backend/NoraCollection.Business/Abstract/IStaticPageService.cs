using System;
using NoraCollection.Shared.Dtos.ResponseDtos;
using NoraCollection.Shared.Dtos.StaticPageDtos;

namespace NoraCollection.Business.Abstract;

public interface IStaticPageService
{
  Task<ResponseDto<StaticPageDto>> GetByIdAsync(int id);
  Task<ResponseDto<StaticPageDto>> GetBySlugAsync(string slug);
  Task<ResponseDto<IEnumerable<StaticPageDto>>> GetAllAsync(bool? isPublished = null);
  Task<ResponseDto<IEnumerable<StaticPageDto>>> GetFooterPagesAsync();
  Task<ResponseDto<StaticPageDto>> AddAsync(StaticPageCreateDto staticPageCreateDto);
  Task<ResponseDto<NoContentDto>> UpdateAsync(StaticPageUpdateDto staticPageUpdateDto);
  Task<ResponseDto<NoContentDto>> DeleteAsync(int id);
  Task<ResponseDto<NoContentDto>> PublishAsync(int id);
  Task<ResponseDto<NoContentDto>> UnPublishAsync(int id);
}
