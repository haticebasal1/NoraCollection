using System;
using NoraCollection.Shared.Dtos.ResponseDtos;
using NoraCollection.Shared.Dtos.StoneTypeDtos;

namespace NoraCollection.Business.Abstract;

public interface IStoneTypeService
{
  Task<ResponseDto<StoneTypeDto>>GetAsync(int id);
  Task<ResponseDto<IEnumerable<StoneTypeDto>>>GetAllAsync(bool? isDeleted = false,bool? isActive= null);
  Task<ResponseDto<int>> CountAsync(bool? isDeleted = false, bool? isActive = null);
  Task<ResponseDto<StoneTypeDto>> AddAsync(StoneTypeCreateDto stoneTypeCreateDto);
  Task<ResponseDto<NoContentDto>> UpdateAsync (StoneTypeUpdateDto stoneTypeUpdateDto);
  Task<ResponseDto<NoContentDto>> HardDeleteAsync(int id);
  Task<ResponseDto<NoContentDto>> SoftDeleteAsync(int id);
  
  // ✅ Taş tipinin aktif/pasif durumunu toggle eder (IsActive)
  Task<ResponseDto<NoContentDto>> ToggleIsActiveAsync(int id);
}





