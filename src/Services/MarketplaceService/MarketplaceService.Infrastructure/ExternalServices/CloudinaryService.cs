using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MarketplaceService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MarketplaceService.Infrastructure.ExternalServices;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<CloudinaryService> _logger;

    public CloudinaryService(IConfiguration configuration, ILogger<CloudinaryService> logger)
    {
        _logger = logger;

        var cloudName = configuration["Cloudinary:CloudName"]!;
        var apiKey = configuration["Cloudinary:ApiKey"]!;
        var apiSecret = configuration["Cloudinary:ApiSecret"]!;

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, imageStream),
            Folder = "buyless/marketplace",
            Transformation = new Transformation()
                .Width(800)
                .Height(800)
                .Crop("limit")
                .Quality("auto")
                .FetchFormat("auto")
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error is not null)
        {
            _logger.LogError("Cloudinary yükleme hatası: {Error}", result.Error.Message);
            throw new InvalidOperationException($"Fotoğraf yüklenemedi: {result.Error.Message}");
        }

        return result.SecureUrl.ToString();
    }

    public async Task DeleteImageAsync(string imageUrl)
    {
        try
        {
            // Cloudinary URL'inden public_id'yi çıkar
            var uri = new Uri(imageUrl);
            var segments = uri.AbsolutePath.Split('/');
            var publicIdWithExtension = string.Join("/", segments.SkipWhile(s => s != "buyless").ToArray());
            var publicId = Path.GetFileNameWithoutExtension(publicIdWithExtension);

            var deleteParams = new DeletionParams($"buyless/marketplace/{publicId}");
            await _cloudinary.DestroyAsync(deleteParams);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cloudinary'den fotoğraf silinirken hata oluştu: {Url}", imageUrl);
        }
    }
}