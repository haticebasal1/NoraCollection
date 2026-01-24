using System;
using NoraCollection.Shared.Dtos.ColorDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IColorService
{
   Task<ResponseDto<ColorDto>> GetAsync(int id);
   Task<ResponseDto<IEnumerable<ColorDto>>> GetAllAsync(bool? isDeleted=false,bool? isActive=null);
   Task<ResponseDto<int>> CountAsync(bool? isDeleted=false,bool? isActive=null);
   Task<ResponseDto<ColorDto>> AddAsync(ColorCreateDto colorCreateDto);
   Task<ResponseDto<NoContentDto>> HardDeleteAsync(int id);
   Task<ResponseDto<NoContentDto>> UpdateAsync(ColorUpdateDto colorUpdateDto);
   Task<ResponseDto<NoContentDto>> SoftDeleteAsync(int id);
   Task<ResponseDto<NoContentDto>> ToggleIsActiveAsync(int id);
}


