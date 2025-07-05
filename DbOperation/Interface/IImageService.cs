using DbOperation.Models;

using System.Collections.Generic;
using System.Threading.Tasks;
using static DbOperation.Implementation.ImageServices;

namespace DbOperation.Interface
{
    public interface IImageServices
    {
        // Basic CRUD operations
        Task<CarImages> GetImageByIdAsync(int carImageId);
        Task<List<CarImageDisplayDto>> GetAllImagesAsync();
        Task<CarImageDisplayDto> GetImageForEditAsync(int carImageId);
        Task<bool> AddOrUpdateImageAsync(CarImageDto imageDto);
        Task<bool> DeleteImageAsync(int carImageId);

        // Specific operations
        Task<CarImages> GetImageByCarRegistrationAsync(int listingId);
        Task<bool> HasImagesForCarAsync(int listingId);

        // Utility methods
        string ConvertByteArrayToBase64(byte[] imageData);
        byte[] ConvertBase64ToByteArray(string base64String);
        string GenerateImagePath(byte[] imageData, int carImageId, int slotNumber);
    }
}