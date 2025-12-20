using System;
using NoraCollection.Shared.Dtos.ColorDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IColorService
{
    Task<ResponseDto<ColorDto>> GetByIdasync(int id);
    Task<ResponseDto<ColorDto>> GetBySlugAsync(string slug);
    Task<ResponseDto<IEnumerable<ColorDto>>> GetAllAsync();
    Task<ResponseDto<ColorDto>> AddAsync(ColorCreateDto colorCreateDto);
    Task<ResponseDto<NoContentDto>> UpdateAsync(ColorUpdateDto colorUpdateDto);
    Task<ResponseDto<NoContentDto>> DeleteAsync(int id);
    Task<ResponseDto<int>> CountAsync();
}
