using System;
using Microsoft.AspNetCore.Http;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Abstract;

public interface IImageService
{
    Task<ResponseDto<string>> UploadAsync(IFormFile image, string folderName);
    Task<ResponseDto<List<string>>> UploadMultipleAsync(List<IFormFile> images, string folderName);
    Task<ResponseDto<string>> ResizeAndUploadAsync(IFormFile image, string folderName);
    ResponseDto<NoContentDto> DeleteImage(string imageUrl);
    ResponseDto<bool> ImageExists(string imageUrl);
}
