namespace Booking.Application.Interfaces
{
    public interface IImageService
    {
        Task<string> GetImageUrl(string fileName);
        Task UploadImage(string imageName, Stream fileStream);
        Task DeleteImage(string filePath);
    }
}
