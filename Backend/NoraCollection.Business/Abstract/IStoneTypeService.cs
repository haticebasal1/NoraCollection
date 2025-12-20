using System;
using NoraCollection.Shared.Dtos.ResponseDtos;
using NoraCollection.Shared.Dtos.StoneTypeDtos;

namespace NoraCollection.Business.Abstract;

public interface IStoneTypeService
{
    Task<ResponseDto<StoneTypeDto>> GetByIdAsync(int id);// Getirme
    Task<ResponseDto<StoneTypeDto>> GetBySlugAsync(string slug);
    Task<ResponseDto<IEnumerable<StoneTypeDto>>> GetAllAsync();
    Task<ResponseDto<StoneTypeDto>> AddAsync(StoneTypeCreateDto stoneTypeCreateDto);// CRUD
    Task<ResponseDto<NoContentDto>> UpdateAsync(StoneTypeUpdateDto stoneTypeUpdateDto);
    Task<ResponseDto<NoContentDto>> DeleteAsync(int id);
    Task<ResponseDto<int>> CountAsync();// Sayım
}
