using System;
using Microsoft.AspNetCore.Http;
using NoraCollection.Business.Abstract;
using NoraCollection.Shared.Dtos.ResponseDtos;

namespace NoraCollection.Business.Concrete;

public class ImageManager : IImageService
{
    private readonly string _imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
    public ResponseDto<NoContentDto> DeleteImage(string imageUrl)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(imageUrl))//boş bir metin gönderilirse
            {
                return ResponseDto<NoContentDto>.Fail("Silinecek resim yolu boş olamaz!", StatusCodes.Status400BadRequest);
            }
            string relativePath = imageUrl;//images/ kısmı atıp categories/dogaltas.jpg bunu koymak için
            if (imageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                var uri = new Uri(imageUrl);
                relativePath = uri.AbsolutePath
                .Replace("/images/", "")
                .TrimStart('/');
            }
            //hala bizim wwwroot/images klasörümüzün içinde mi?". Eğer dışına çıkmaya çalışıyorsa "Erişim Reddedildi" (403) hatası verir.
            var combinedPath = Path.Combine(_imageFolderPath, relativePath);
            var fullPath = Path.GetFullPath(combinedPath);
            if (!fullPath.StartsWith(_imageFolderPath, StringComparison.OrdinalIgnoreCase))
            {
                return ResponseDto<NoContentDto>.Fail("Geçersiz dosya yolu erişimi!", StatusCodes.Status403Forbidden);
            }
            if (File.Exists(fullPath))//fiziksel silme işlemi
            {
                File.Delete(fullPath);
            }
            return ResponseDto<NoContentDto>.Success(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<NoContentDto>.Fail(ex.Message, StatusCodes.Status500InternalServerError);
        }
    }

    public ResponseDto<bool> ImageExists(string imageUrl)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return ResponseDto<bool>.Success(false, StatusCodes.Status200OK);
            }
            string relativePath = imageUrl;
            if (imageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                var uri = new Uri(imageUrl);
                relativePath = uri.AbsolutePath
                .Replace("/images/", "")
                .TrimStart('/');
            }
            //hala bizim wwwroot/images klasörümüzün içinde mi?". Eğer dışına çıkmaya çalışıyorsa "Erişim Reddedildi" (403) hatası verir.
            var combinedPath = Path.Combine(_imageFolderPath, relativePath);
            var fullPath = Path.GetFullPath(combinedPath);
            if (!fullPath.StartsWith(_imageFolderPath, StringComparison.OrdinalIgnoreCase))
            {
                return ResponseDto<bool>.Fail("Geçersiz dosya yolu erişimi!", StatusCodes.Status403Forbidden);
            }
            var exists= File.Exists(fullPath);
            return ResponseDto<bool>.Success(exists,StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail($"Dosya kontrolü sırasında hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
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
