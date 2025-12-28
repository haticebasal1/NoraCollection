using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
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
            var exists = File.Exists(fullPath);
            return ResponseDto<bool>.Success(exists, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return ResponseDto<bool>.Fail($"Dosya kontrolü sırasında hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<string>> ResizeAndUploadAsync(IFormFile image, string folderName)
    {
        try
        {
            // Dosya null mı veya boş mu kontrolü
            if (image is null || image.Length == 0)
            {
                return ResponseDto<string>.Fail("Dosya boş olmaz!", StatusCodes.Status400BadRequest);
            }
            // Dosya boyutunun 10 mb tan büyük mü kontrolü
            if (image.Length > 10 * 1024 * 1024)
            {
                return ResponseDto<string>.Fail("Dosya boyutu 10MB'dan büyük olamaz!", StatusCodes.Status400BadRequest);
            }
            // Dosya uzantısı kontrolü (Güvenlik)
            string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
            // Yüklenen dosyanın uzantısını alıyoruz
            var imageExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
            // İzin verilmeyen bir uzantı varsa işlemi durdur
            if (!allowedExtensions.Contains(imageExtension))
            {
                return ResponseDto<string>.Fail("Sadece jpg, jpeg, png, webp dosyaları yüklenebilir!", StatusCodes.Status400BadRequest);
            }
            //Resmin kaydedileceği klasörü oluştur
            var folderPath = Path.Combine(_imageFolderPath, folderName);
            // Klasör yoksa oluştur
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            // Benzersiz dosya adı oluştur
            var fileName = $"{Guid.NewGuid()}{imageExtension}";
            var fileFullPath = Path.Combine(folderPath, fileName);
            //Resmi oku, resize et ve kaydet
            using (var inputStream = image.OpenReadStream())
            using (var imageSharp = await Image.LoadAsync(inputStream))
            {
                int maxWidth = 800;
                int maxHeight = 800;
                // En-boy oranını bozmadan resize
                imageSharp.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(maxWidth, maxHeight)
                }));
                // JPEG encoder ayarları
                // %85 kalite → gözle fark yok, dosya boyutu düşük
                var encoder = new JpegEncoder
                {
                    Quality = 85
                };
                // Resmi diske kaydet
                await imageSharp.SaveAsync(fileFullPath, encoder);
            }
            //Veritabanına kaydedilecek relative path
            var imageUrl = $"images/{folderName}/{fileName}";

            return ResponseDto<string>.Success(imageUrl, StatusCodes.Status200OK);

        }
        catch (Exception ex)
        {
            return ResponseDto<string>.Fail($"Resim işlenirken hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<string>> UploadAsync(IFormFile image, string folderName)
    {
        try
        {
            // Dosya null mı veya boş mu kontrolü
            if (image is null || image.Length == 0)
            {
                return ResponseDto<string>.Fail("Dosya boş olmaz!", StatusCodes.Status400BadRequest);
            }
            // Dosya boyutunun 10 mb tan büyük mü kontrolü
            if (image.Length > 10 * 1024 * 1024)
            {
                return ResponseDto<string>.Fail("Dosya boyutu 10MB'dan büyük olamaz!", StatusCodes.Status400BadRequest);
            }
            // Dosya uzantısı kontrolü (Güvenlik)
            string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
            // Yüklenen dosyanın uzantısını alıyoruz
            var imageExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
            // İzin verilmeyen bir uzantı varsa işlemi durdur
            if (!allowedExtensions.Contains(imageExtension))
            {
                return ResponseDto<string>.Fail("Sadece jpg, jpeg, png, webp dosyaları yüklenebilir!", StatusCodes.Status400BadRequest);
            }
            //Resmin kaydedileceği klasörü oluştur
            var folderPath = Path.Combine(_imageFolderPath, folderName);
            // Klasör yoksa oluştur
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            // Benzersiz dosya adı oluştur
            var fileName = $"{Guid.NewGuid()}{imageExtension}";
            var fileFullPath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(fileFullPath, FileMode.Create);
            await image.CopyToAsync(stream);
            var imageUrl = $"images/{folderName}/{fileName}";
            return ResponseDto<string>.Success(imageUrl, StatusCodes.Status201Created);

        }
        catch (Exception ex)
        {
            return ResponseDto<string>.Fail($"Beklenmedik Hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto<List<string>>> UploadMultipleAsync(List<IFormFile> images, string folderName)
    {
        try
        {
            if (images is null || images.Count == 0)
            {
                return ResponseDto<List<string>>.Fail("Resim gönderilemedi!",StatusCodes.Status400BadRequest);
            }

            var uploadedUrls= new List<string>();

            foreach (var image in images)
            {
                var result = await UploadAsync(image,folderName);
                if (result.IsSuccessful)
                {
                    uploadedUrls.Add(result.Data);
                }
            }
            if (uploadedUrls.Count == 0)
            {
                return ResponseDto<List<string>>.Fail("Hiçbir dosya yüklenemedi!",StatusCodes.Status400BadRequest);
            }
            return ResponseDto<List<string>>.Success(uploadedUrls,StatusCodes.Status201Created);
        }
         catch (Exception ex)
        {
            return ResponseDto<List<string>>.Fail($"Hata:{ex.Message}", StatusCodes.Status500InternalServerError);
        }
    }
}
