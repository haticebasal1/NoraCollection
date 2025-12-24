using System;
using Microsoft.AspNetCore.Http;
using NoraCollection.Business.Abstract;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Concrete;

public class ImageManager : IImageService
{
    public ResponseDto<NoContentDto> DeleteImage(string imageUrl)
    {
        throw new NotImplementedException();
    }

    public ResponseDto<bool> ImageExists(string imageUrl)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<string>> ResizeAndUploadAsync(IFormFile image, string folderName)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<string>> UploadAsync(IFormFile image, string folderName)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseDto<List<string>>> UploadMultipleAsync(List<IFormFile> image, string folderName)
    {
        throw new NotImplementedException();
    }
}
