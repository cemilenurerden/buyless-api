namespace MarketplaceService.Application.Interfaces;

public interface ICloudinaryService
{
    /// <summary>
    /// Bir görseli Cloudinary'ye yükler ve dönen URL'i döndürür.
    /// </summary>
    Task<string> UploadImageAsync(Stream imageStream, string fileName);

    /// <summary>
    /// Cloudinary'deki bir görseli siler.
    /// </summary>
    Task DeleteImageAsync(string imageUrl);
}