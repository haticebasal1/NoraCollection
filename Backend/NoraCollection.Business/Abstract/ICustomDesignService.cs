using System;
using NoraCollection.Shared.Dtos.CustomDesignDtos;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface ICustomDesignService
{
Task<ResponseDto<CustomDesignDto>> GetByIdAsync(int id);
Task<ResponseDto<IEnumerable<CustomDesignDto>>> GetAllAsync(string ? status=null);
Task<ResponseDto<IEnumerable<CustomDesignDto>>> GetByUserAsync(string userId);
Task<ResponseDto<CustomDesignDto>> AddAsync(CustomDesignCreateDto customDesignCreateDto);// Müşteri talep oluşturur
Task<ResponseDto<NoContentDto>> UpdateAsync(CustomDesignUpdateDto customDesignUpdateDto);// Admin günceller
Task<ResponseDto<NoContentDto>> DeleteAsync(int id);

// Durum Güncelleme (Admin)
Task<ResponseDto<NoContentDto>> ApproveAsync(int id,decimal price,string? adminNote);//Onay
Task<ResponseDto<NoContentDto>> RejectAsync(int id,string reason);//Reddet
Task<ResponseDto<NoContentDto>> CompletedAsync( int id);//Tasarım tamamlandı, teslim edildi
Task<ResponseDto<int>> CountAsync(string ? status = null); 
}
