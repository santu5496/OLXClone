using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DbOperation.Implementation
{
    public class ImageServices : IImageServices
    {
        private readonly DbContextOptions<Assignment4Context> _dbConn;

        public ImageServices(string dbConn)
        {
            _dbConn = new DbContextOptionsBuilder<Assignment4Context>()
                .UseSqlServer(dbConn)
                .Options;
        }

        public async Task<CarImages> GetImageByIdAsync(int id)
        {
            using var context = new Assignment4Context(_dbConn);
            return await context.CarImages.FindAsync(id);
        }

        public async Task<List<CarImages>> GetAllImagesAsync()
        {
            using var context = new Assignment4Context(_dbConn);
            return await context.CarImages.ToListAsync();
        }

        public async Task AddImageAsync(CarImages image)
        {
            using var context = new Assignment4Context(_dbConn);
            context.CarImages.Add(image);
            await context.SaveChangesAsync();
        }

        public async Task UpdateImageAsync(CarImages image)
        {
            using var context = new Assignment4Context(_dbConn);
            context.CarImages.Update(image);
            await context.SaveChangesAsync();
        }

        public async Task DeleteImageAsync(int id)
        {
            using var context = new Assignment4Context(_dbConn);
            var image = await context.CarImages.FindAsync(id);
            if (image != null)
            {
                context.CarImages.Remove(image);
                await context.SaveChangesAsync();
            }
        }

        // 🆕 Optional: Save image from stream/file into Slot1
        public async Task AddImageFromStreamAsync(int listingId, string registrationNumber, Stream imageStream, string description)
        {
            using var context = new Assignment4Context(_dbConn);
            using var ms = new MemoryStream();
            await imageStream.CopyToAsync(ms);
            var imageBytes = ms.ToArray();

            var newImage = new CarImages
            {
                listingId = listingId,
                registrationNumber = registrationNumber,
                slot1ImageData = imageBytes,
                slot1Description = description,
                uploadDate = DateTime.UtcNow,
                isActive = true
            };

            context.CarImages.Add(newImage);
            await context.SaveChangesAsync();
        }
    }
}
