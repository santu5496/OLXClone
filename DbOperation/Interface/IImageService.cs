using DbOperation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.Interface
{
    public interface IImageServices
    {
        Task<CarImages> GetImageByIdAsync(int id);
        Task<List<CarImages>> GetAllImagesAsync();
        Task AddImageAsync(CarImages image);
        Task UpdateImageAsync(CarImages image);
        Task DeleteImageAsync(int id);

        // ✅ Upload image directly from stream (e.g. from file upload)
        Task AddImageFromStreamAsync(int listingId, string registrationNumber, Stream imageStream, string description);


    }
}
