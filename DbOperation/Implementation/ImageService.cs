using DbOperation.Interface;
using DbOperation.Models;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<CarImages> GetImageByIdAsync(int carImageId)
        {
            using var context = new Assignment4Context(_dbConn);
            return await context.CarImages
                .Include(ci => ci.listing)
                .FirstOrDefaultAsync(ci => ci.carImageId == carImageId);
        }

        public async Task<List<CarImageDisplayDto>> GetAllImagesAsync()
        {
            using var context = new Assignment4Context(_dbConn);
            var images = await context.CarImages
                .Include(ci => ci.listing)
                .Where(ci => ci.isActive == true)
                .OrderByDescending(ci => ci.uploadDate)
                .ToListAsync();

            var result = new List<CarImageDisplayDto>();

            foreach (var image in images)
            {
                var displayDto = new CarImageDisplayDto
                {
                    carImageId = image.carImageId,
                    carRegistrationId = image.listingId,
                    registrationNumber = image.registrationNumber,
                    uploadDate = image.uploadDate,
                    isActive = image.isActive,

                    // Convert byte arrays to base64 image paths for display
                    slot1ImagePath = GenerateImagePath(image.slot1ImageData, image.carImageId, 1),
                    slot1Description = image.slot1Description,
                    slot2ImagePath = GenerateImagePath(image.slot2ImageData, image.carImageId, 2),
                    slot2Description = image.slot2Description,
                    slot3ImagePath = GenerateImagePath(image.slot3ImageData, image.carImageId, 3),
                    slot3Description = image.slot3Description,
                    slot4ImagePath = GenerateImagePath(image.slot4ImageData, image.carImageId, 4),
                    slot4Description = image.slot4Description,
                    slot5ImagePath = GenerateImagePath(image.slot5ImageData, image.carImageId, 5),
                    slot5Description = image.slot5Description,
                    slot6ImagePath = GenerateImagePath(image.slot6ImageData, image.carImageId, 6),
                    slot6Description = image.slot6Description,
                    slot7ImagePath = GenerateImagePath(image.slot7ImageData, image.carImageId, 7),
                    slot7Description = image.slot7Description,
                    slot8ImagePath = GenerateImagePath(image.slot8ImageData, image.carImageId, 8),
                    slot8Description = image.slot8Description,
                    slot9ImagePath = GenerateImagePath(image.slot9ImageData, image.carImageId, 9),
                    slot9Description = image.slot9Description,
                    slot10ImagePath = GenerateImagePath(image.slot10ImageData, image.carImageId, 10),
                    slot10Description = image.slot10Description
                };

                result.Add(displayDto);
            }

            return result;
        }

        public async Task<CarImageDisplayDto> GetImageForEditAsync(int carImageId)
        {
            using var context = new Assignment4Context(_dbConn);
            var image = await context.CarImages
                .Include(ci => ci.listing)
                .FirstOrDefaultAsync(ci => ci.carImageId == carImageId);

            if (image == null) return null;

            return new CarImageDisplayDto
            {
                carImageId = image.carImageId,
                carRegistrationId = image.listingId,
                registrationNumber = image.registrationNumber,
                uploadDate = image.uploadDate,
                isActive = image.isActive,

                // Convert byte arrays to base64 image paths for editing
                slot1ImagePath = GenerateImagePath(image.slot1ImageData, image.carImageId, 1),
                slot1Description = image.slot1Description,
                slot2ImagePath = GenerateImagePath(image.slot2ImageData, image.carImageId, 2),
                slot2Description = image.slot2Description,
                slot3ImagePath = GenerateImagePath(image.slot3ImageData, image.carImageId, 3),
                slot3Description = image.slot3Description,
                slot4ImagePath = GenerateImagePath(image.slot4ImageData, image.carImageId, 4),
                slot4Description = image.slot4Description,
                slot5ImagePath = GenerateImagePath(image.slot5ImageData, image.carImageId, 5),
                slot5Description = image.slot5Description,
                slot6ImagePath = GenerateImagePath(image.slot6ImageData, image.carImageId, 6),
                slot6Description = image.slot6Description,
                slot7ImagePath = GenerateImagePath(image.slot7ImageData, image.carImageId, 7),
                slot7Description = image.slot7Description,
                slot8ImagePath = GenerateImagePath(image.slot8ImageData, image.carImageId, 8),
                slot8Description = image.slot8Description,
                slot9ImagePath = GenerateImagePath(image.slot9ImageData, image.carImageId, 9),
                slot9Description = image.slot9Description,
                slot10ImagePath = GenerateImagePath(image.slot10ImageData, image.carImageId, 10),
                slot10Description = image.slot10Description
            };
        }


        // Fixed AddOrUpdateImageAsync method for your exact database structure
        public async Task<bool> AddOrUpdateImageAsync1(CarImageDto imageDto)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                CarImages carImage;
                bool isNewRecord = imageDto.carImageId == 0;

                if (isNewRecord) // Add new record
                {
                    carImage = new CarImages
                    {
                        listingId = imageDto.carRegistrationId,
                        registrationNumber = imageDto.registrationNumber,
                        uploadDate = DateTime.UtcNow,
                        isActive = imageDto.isActive
                    };

                    // Convert base64 strings to byte arrays for all 15 slots
                    // Slots 1-10 (from frontend)
                    carImage.slot1ImageData = ConvertBase64ToByteArray(imageDto.slot1ImageData);
                    carImage.slot1Description = imageDto.slot1Description;
                    carImage.slot2ImageData = ConvertBase64ToByteArray(imageDto.slot2ImageData);
                    carImage.slot2Description = imageDto.slot2Description;
                    carImage.slot3ImageData = ConvertBase64ToByteArray(imageDto.slot3ImageData);
                    carImage.slot3Description = imageDto.slot3Description;
                    carImage.slot4ImageData = ConvertBase64ToByteArray(imageDto.slot4ImageData);
                    carImage.slot4Description = imageDto.slot4Description;
                    carImage.slot5ImageData = ConvertBase64ToByteArray(imageDto.slot5ImageData);
                    carImage.slot5Description = imageDto.slot5Description;
                    carImage.slot6ImageData = ConvertBase64ToByteArray(imageDto.slot6ImageData);
                    carImage.slot6Description = imageDto.slot6Description;
                    carImage.slot7ImageData = ConvertBase64ToByteArray(imageDto.slot7ImageData);
                    carImage.slot7Description = imageDto.slot7Description;
                    carImage.slot8ImageData = ConvertBase64ToByteArray(imageDto.slot8ImageData);
                    carImage.slot8Description = imageDto.slot8Description;
                    carImage.slot9ImageData = ConvertBase64ToByteArray(imageDto.slot9ImageData);
                    carImage.slot9Description = imageDto.slot9Description;
                    carImage.slot10ImageData = ConvertBase64ToByteArray(imageDto.slot10ImageData);
                    carImage.slot10Description = imageDto.slot10Description;

                    // Slots 11-15 (keep null for now as frontend doesn't use them)
                    carImage.slot11ImageData = null;
                    carImage.slot11Description = null;
                    carImage.slot12ImageData = null;
                    carImage.slot12Description = null;
                    carImage.slot13ImageData = null;
                    carImage.slot13Description = null;
                    carImage.slot14ImageData = null;
                    carImage.slot14Description = null;
                    carImage.slot15ImageData = null;
                    carImage.slot15Description = null;

                    // Add new record to context
                    context.CarImages.Add(carImage);
                }
                else // Update existing record
                {
                    carImage = await context.CarImages.FindAsync(imageDto.carImageId);
                    if (carImage == null) return false;

                    // Update basic fields
                    carImage.listingId = imageDto.carRegistrationId;
                    carImage.registrationNumber = imageDto.registrationNumber;
                    carImage.isActive = imageDto.isActive;

                    // Update image data - only update if new data is provided
                    if (!string.IsNullOrEmpty(imageDto.slot1ImageData))
                        carImage.slot1ImageData = ConvertBase64ToByteArray(imageDto.slot1ImageData);
                    carImage.slot1Description = imageDto.slot1Description;

                    if (!string.IsNullOrEmpty(imageDto.slot2ImageData))
                        carImage.slot2ImageData = ConvertBase64ToByteArray(imageDto.slot2ImageData);
                    carImage.slot2Description = imageDto.slot2Description;

                    if (!string.IsNullOrEmpty(imageDto.slot3ImageData))
                        carImage.slot3ImageData = ConvertBase64ToByteArray(imageDto.slot3ImageData);
                    carImage.slot3Description = imageDto.slot3Description;

                    if (!string.IsNullOrEmpty(imageDto.slot4ImageData))
                        carImage.slot4ImageData = ConvertBase64ToByteArray(imageDto.slot4ImageData);
                    carImage.slot4Description = imageDto.slot4Description;

                    if (!string.IsNullOrEmpty(imageDto.slot5ImageData))
                        carImage.slot5ImageData = ConvertBase64ToByteArray(imageDto.slot5ImageData);
                    carImage.slot5Description = imageDto.slot5Description;

                    if (!string.IsNullOrEmpty(imageDto.slot6ImageData))
                        carImage.slot6ImageData = ConvertBase64ToByteArray(imageDto.slot6ImageData);
                    carImage.slot6Description = imageDto.slot6Description;

                    if (!string.IsNullOrEmpty(imageDto.slot7ImageData))
                        carImage.slot7ImageData = ConvertBase64ToByteArray(imageDto.slot7ImageData);
                    carImage.slot7Description = imageDto.slot7Description;

                    if (!string.IsNullOrEmpty(imageDto.slot8ImageData))
                        carImage.slot8ImageData = ConvertBase64ToByteArray(imageDto.slot8ImageData);
                    carImage.slot8Description = imageDto.slot8Description;

                    if (!string.IsNullOrEmpty(imageDto.slot9ImageData))
                        carImage.slot9ImageData = ConvertBase64ToByteArray(imageDto.slot9ImageData);
                    carImage.slot9Description = imageDto.slot9Description;

                    if (!string.IsNullOrEmpty(imageDto.slot10ImageData))
                        carImage.slot10ImageData = ConvertBase64ToByteArray(imageDto.slot10ImageData);
                    carImage.slot10Description = imageDto.slot10Description;

                    // No need to call context.CarImages.Add() for updates
                    // EF Core is already tracking the entity
                }

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddOrUpdateImageAsync: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return false;
            }
        }


        // Enhanced AddOrUpdateImageAsync with comprehensive error handling and debugging
        public async Task<bool> AddOrUpdateImageAsync(CarImageDto imageDto)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                CarImages carImage;
                bool isNewRecord = imageDto.carImageId == 0;

                // STEP 1: Validation before processing
                if (imageDto.carRegistrationId <= 0)
                {
                    Console.WriteLine("Error: Invalid carRegistrationId");
                    return false;
                }

                if (string.IsNullOrEmpty(imageDto.registrationNumber))
                {
                    Console.WriteLine("Error: registrationNumber is required");
                    return false;
                }

                // STEP 2: Verify that the car listing exists
                var carListing = await context.CarListings.FirstOrDefaultAsync(cl => cl.listingId == imageDto.carRegistrationId);
                if (carListing == null)
                {
                    Console.WriteLine($"Error: Car listing with ID {imageDto.carRegistrationId} not found");
                    return false;
                }

                if (isNewRecord) // Add new record
                {
                    Console.WriteLine("Creating new car image record...");

                    // STEP 3: Check if images already exist for this car
                    var existingImage = await context.CarImages
                        .FirstOrDefaultAsync(ci => ci.listingId == imageDto.carRegistrationId && ci.isActive == true);

                    if (existingImage != null)
                    {
                        Console.WriteLine($"Warning: Images already exist for car {imageDto.carRegistrationId}. Consider updating instead.");
                        // You can either return false or proceed with creating another record
                        // return false;
                    }

                    carImage = new CarImages
                    {
                        listingId = imageDto.carRegistrationId,
                        registrationNumber = imageDto.registrationNumber,
                        uploadDate = DateTime.UtcNow,
                        isActive = true // Ensure it's always true for new records
                    };

                    // STEP 4: Convert and validate image data
                    try
                    {
                        carImage.slot1ImageData = ConvertAndValidateBase64(imageDto.slot1ImageData, "slot1");
                        carImage.slot1Description = imageDto.slot1Description ?? "";
                        carImage.slot2ImageData = ConvertAndValidateBase64(imageDto.slot2ImageData, "slot2");
                        carImage.slot2Description = imageDto.slot2Description ?? "";
                        carImage.slot3ImageData = ConvertAndValidateBase64(imageDto.slot3ImageData, "slot3");
                        carImage.slot3Description = imageDto.slot3Description ?? "";
                        carImage.slot4ImageData = ConvertAndValidateBase64(imageDto.slot4ImageData, "slot4");
                        carImage.slot4Description = imageDto.slot4Description ?? "";
                        carImage.slot5ImageData = ConvertAndValidateBase64(imageDto.slot5ImageData, "slot5");
                        carImage.slot5Description = imageDto.slot5Description ?? "";
                        carImage.slot6ImageData = ConvertAndValidateBase64(imageDto.slot6ImageData, "slot6");
                        carImage.slot6Description = imageDto.slot6Description ?? "";
                        carImage.slot7ImageData = ConvertAndValidateBase64(imageDto.slot7ImageData, "slot7");
                        carImage.slot7Description = imageDto.slot7Description ?? "";
                        carImage.slot8ImageData = ConvertAndValidateBase64(imageDto.slot8ImageData, "slot8");
                        carImage.slot8Description = imageDto.slot8Description ?? "";
                        carImage.slot9ImageData = ConvertAndValidateBase64(imageDto.slot9ImageData, "slot9");
                        carImage.slot9Description = imageDto.slot9Description ?? "";
                        carImage.slot10ImageData = ConvertAndValidateBase64(imageDto.slot10ImageData, "slot10");
                        carImage.slot10Description = imageDto.slot10Description ?? "";

                        // Initialize unused slots to avoid null reference issues
                        carImage.slot11ImageData = null;
                        carImage.slot11Description = "";
                        carImage.slot12ImageData = null;
                        carImage.slot12Description = "";
                        carImage.slot13ImageData = null;
                        carImage.slot13Description = "";
                        carImage.slot14ImageData = null;
                        carImage.slot14Description = "";
                        carImage.slot15ImageData = null;
                        carImage.slot15Description = "";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error converting image data: {ex.Message}");
                        return false;
                    }

                    context.CarImages.Add(carImage);
                    Console.WriteLine("New car image record added to context");
                }
                else // Update existing record
                {
                    Console.WriteLine($"Updating existing car image record with ID: {imageDto.carImageId}");

                    carImage = await context.CarImages.FindAsync(imageDto.carImageId);
                    if (carImage == null)
                    {
                        Console.WriteLine($"Error: Car image with ID {imageDto.carImageId} not found");
                        return false;
                    }

                    // Update basic fields
                    carImage.listingId = imageDto.carRegistrationId;
                    carImage.registrationNumber = imageDto.registrationNumber;
                    carImage.isActive = imageDto.isActive;

                    // Update image data - only if new data is provided, otherwise keep existing
                    if (!string.IsNullOrEmpty(imageDto.slot1ImageData))
                        carImage.slot1ImageData = ConvertAndValidateBase64(imageDto.slot1ImageData, "slot1");
                    if (imageDto.slot1Description != null)
                        carImage.slot1Description = imageDto.slot1Description;

                    if (!string.IsNullOrEmpty(imageDto.slot2ImageData))
                        carImage.slot2ImageData = ConvertAndValidateBase64(imageDto.slot2ImageData, "slot2");
                    if (imageDto.slot2Description != null)
                        carImage.slot2Description = imageDto.slot2Description;

                    if (!string.IsNullOrEmpty(imageDto.slot3ImageData))
                        carImage.slot3ImageData = ConvertAndValidateBase64(imageDto.slot3ImageData, "slot3");
                    if (imageDto.slot3Description != null)
                        carImage.slot3Description = imageDto.slot3Description;

                    if (!string.IsNullOrEmpty(imageDto.slot4ImageData))
                        carImage.slot4ImageData = ConvertAndValidateBase64(imageDto.slot4ImageData, "slot4");
                    if (imageDto.slot4Description != null)
                        carImage.slot4Description = imageDto.slot4Description;

                    if (!string.IsNullOrEmpty(imageDto.slot5ImageData))
                        carImage.slot5ImageData = ConvertAndValidateBase64(imageDto.slot5ImageData, "slot5");
                    if (imageDto.slot5Description != null)
                        carImage.slot5Description = imageDto.slot5Description;

                    if (!string.IsNullOrEmpty(imageDto.slot6ImageData))
                        carImage.slot6ImageData = ConvertAndValidateBase64(imageDto.slot6ImageData, "slot6");
                    if (imageDto.slot6Description != null)
                        carImage.slot6Description = imageDto.slot6Description;

                    if (!string.IsNullOrEmpty(imageDto.slot7ImageData))
                        carImage.slot7ImageData = ConvertAndValidateBase64(imageDto.slot7ImageData, "slot7");
                    if (imageDto.slot7Description != null)
                        carImage.slot7Description = imageDto.slot7Description;

                    if (!string.IsNullOrEmpty(imageDto.slot8ImageData))
                        carImage.slot8ImageData = ConvertAndValidateBase64(imageDto.slot8ImageData, "slot8");
                    if (imageDto.slot8Description != null)
                        carImage.slot8Description = imageDto.slot8Description;

                    if (!string.IsNullOrEmpty(imageDto.slot9ImageData))
                        carImage.slot9ImageData = ConvertAndValidateBase64(imageDto.slot9ImageData, "slot9");
                    if (imageDto.slot9Description != null)
                        carImage.slot9Description = imageDto.slot9Description;

                    if (!string.IsNullOrEmpty(imageDto.slot10ImageData))
                        carImage.slot10ImageData = ConvertAndValidateBase64(imageDto.slot10ImageData, "slot10");
                    if (imageDto.slot10Description != null)
                        carImage.slot10Description = imageDto.slot10Description;

                    Console.WriteLine("Car image record updated in context");
                }

                // STEP 5: Save changes with detailed error handling
                Console.WriteLine("Attempting to save changes to database...");

                try
                {
                    var result = await context.SaveChangesAsync();
                    Console.WriteLine($"Successfully saved {result} changes to database");
                    return true;
                }
                catch (DbUpdateException dbEx)
                {
                    Console.WriteLine($"Database Update Error: {dbEx.Message}");
                    if (dbEx.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {dbEx.InnerException.Message}");

                        // Check for specific SQL Server errors
                        if (dbEx.InnerException.Message.Contains("FOREIGN KEY constraint"))
                        {
                            Console.WriteLine("Error: Foreign key constraint violation. Check if the carRegistrationId exists in CarListings table.");
                        }
                        else if (dbEx.InnerException.Message.Contains("PRIMARY KEY constraint"))
                        {
                            Console.WriteLine("Error: Primary key constraint violation. Duplicate carImageId.");
                        }
                        else if (dbEx.InnerException.Message.Contains("cannot be null"))
                        {
                            Console.WriteLine("Error: Required field is null. Check database schema for NOT NULL constraints.");
                        }
                        else if (dbEx.InnerException.Message.Contains("String or binary data would be truncated"))
                        {
                            Console.WriteLine("Error: Data too long for database field. Check string length limits.");
                        }
                    }
                    return false;
                }
                catch (InvalidOperationException invEx)
                {
                    Console.WriteLine($"Invalid Operation Error: {invEx.Message}");
                    Console.WriteLine("This usually indicates a problem with the DbContext or entity configuration.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error in AddOrUpdateImageAsync: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return false;
            }
        }

        // Enhanced utility method with validation
        private byte[] ConvertAndValidateBase64(string base64String, string slotName)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                Console.WriteLine($"{slotName}: No image data provided");
                return null;
            }

            try
            {
                // Remove data:image prefix if present
                if (base64String.Contains(","))
                {
                    base64String = base64String.Split(',')[1];
                }

                var bytes = Convert.FromBase64String(base64String);

                // Validate image size (e.g., max 10MB per image)
                if (bytes.Length > 10 * 1024 * 1024)
                {
                    Console.WriteLine($"{slotName}: Image too large ({bytes.Length} bytes). Maximum allowed: 10MB");
                    throw new ArgumentException($"Image in {slotName} is too large");
                }

                Console.WriteLine($"{slotName}: Successfully converted {bytes.Length} bytes");
                return bytes;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"{slotName}: Invalid base64 format - {ex.Message}");
                throw new ArgumentException($"Invalid base64 format in {slotName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{slotName}: Error converting base64 - {ex.Message}");
                throw;
            }
        }

        // Enhanced method to check database constraints before saving
        public async Task<bool> ValidateCarImageConstraints(CarImageDto imageDto)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);

                // Check if car listing exists
                var carExists = await context.CarListings
                    .AnyAsync(cl => cl.listingId == imageDto.carRegistrationId);

                if (!carExists)
                {
                    Console.WriteLine($"Validation Error: Car with listingId {imageDto.carRegistrationId} does not exist");
                    return false;
                }

                // Check if updating an existing record
                if (imageDto.carImageId > 0)
                {
                    var existingImage = await context.CarImages
                        .FirstOrDefaultAsync(ci => ci.carImageId == imageDto.carImageId);

                    if (existingImage == null)
                    {
                        Console.WriteLine($"Validation Error: Car image with ID {imageDto.carImageId} does not exist");
                        return false;
                    }
                }

                // Check registration number format if needed
                if (string.IsNullOrEmpty(imageDto.registrationNumber) || imageDto.registrationNumber.Length > 50)
                {
                    Console.WriteLine("Validation Error: Registration number is invalid");
                    return false;
                }

                Console.WriteLine("All validations passed");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during validation: {ex.Message}");
                return false;
            }
        }

        // Method to debug database schema
        public async Task<string> GetDatabaseSchemaInfo()
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);

                var schemaInfo = new System.Text.StringBuilder();

                // Get CarImages table info
                var carImagesEntity = context.Model.FindEntityType(typeof(CarImages));
                if (carImagesEntity != null)
                {
                    schemaInfo.AppendLine("CarImages Table Schema:");
                    schemaInfo.AppendLine($"Table Name: {carImagesEntity.GetTableName()}");

                    foreach (var property in carImagesEntity.GetProperties())
                    {
                        var columnName = property.GetColumnName();
                        var columnType = property.GetColumnType();
                        var isNullable = property.IsNullable;
                        var maxLength = property.GetMaxLength();

                        schemaInfo.AppendLine($"  {columnName}: {columnType}, Nullable: {isNullable}, MaxLength: {maxLength}");
                    }

                    // Get foreign keys
                    foreach (var foreignKey in carImagesEntity.GetForeignKeys())
                    {
                        schemaInfo.AppendLine($"Foreign Key: {foreignKey.Properties.First().Name} -> {foreignKey.PrincipalEntityType.Name}.{foreignKey.PrincipalKey.Properties.First().Name}");
                    }
                }

                return schemaInfo.ToString();
            }
            catch (Exception ex)
            {
                return $"Error getting schema info: {ex.Message}";
            }
        }




        public async Task<bool> DeleteImageAsync(int carImageId)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var image = await context.CarImages.FindAsync(carImageId);

                if (image != null)
                {
                    // Soft delete - just mark as inactive
                    image.isActive = false;
                    await context.SaveChangesAsync();

                    // Or hard delete if preferred:
                    // context.CarImages.Remove(image);
                    // await context.SaveChangesAsync();

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteImageAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<CarImages> GetImageByCarRegistrationAsync(int listingId)
        {
            using var context = new Assignment4Context(_dbConn);
            return await context.CarImages
                .FirstOrDefaultAsync(ci => ci.listingId == listingId && ci.isActive == true);
        }

        public async Task<bool> HasImagesForCarAsync(int listingId)
        {
            using var context = new Assignment4Context(_dbConn);
            return await context.CarImages
                .AnyAsync(ci => ci.listingId == listingId && ci.isActive == true);
        }

        #region Utility Methods

        public string ConvertByteArrayToBase64(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;

            return Convert.ToBase64String(imageData);
        }

        public byte[] ConvertBase64ToByteArray(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return null;

            try
            {
                // Remove data:image prefix if present
                if (base64String.Contains(","))
                {
                    base64String = base64String.Split(',')[1];
                }

                return Convert.FromBase64String(base64String);
            }
            catch
            {
                return null;
            }
        }

        public string GenerateImagePath(byte[] imageData, int carImageId, int slotNumber)
        {
            if (imageData == null || imageData.Length == 0)
                return null;

            // Convert to base64 with data URI prefix for direct display in HTML
            var base64String = Convert.ToBase64String(imageData);
            return $"data:image/jpeg;base64,{base64String}";
        }

        #endregion


        public class CarImageDto
        {
            public int carImageId { get; set; }
            public int carRegistrationId { get; set; }  // Note: This should match listingId
            public string registrationNumber { get; set; }

            // Base64 strings for frontend communication
            public string slot1ImageData { get; set; }
            public string slot1Description { get; set; }
            public string slot2ImageData { get; set; }
            public string slot2Description { get; set; }
            public string slot3ImageData { get; set; }
            public string slot3Description { get; set; }
            public string slot4ImageData { get; set; }
            public string slot4Description { get; set; }
            public string slot5ImageData { get; set; }
            public string slot5Description { get; set; }
            public string slot6ImageData { get; set; }
            public string slot6Description { get; set; }
            public string slot7ImageData { get; set; }
            public string slot7Description { get; set; }
            public string slot8ImageData { get; set; }
            public string slot8Description { get; set; }
            public string slot9ImageData { get; set; }
            public string slot9Description { get; set; }
            public string slot10ImageData { get; set; }
            public string slot10Description { get; set; }

            public DateTime? uploadDate { get; set; }
            public bool isActive { get; set; } = true;
        }

        public class CarImageDisplayDto
        {
            public int carImageId { get; set; }
            public int carRegistrationId { get; set; }
            public string registrationNumber { get; set; }

            // Image paths for display in table
            public string slot1ImagePath { get; set; }
            public string slot1Description { get; set; }
            public string slot2ImagePath { get; set; }
            public string slot2Description { get; set; }
            public string slot3ImagePath { get; set; }
            public string slot3Description { get; set; }
            public string slot4ImagePath { get; set; }
            public string slot4Description { get; set; }
            public string slot5ImagePath { get; set; }
            public string slot5Description { get; set; }
            public string slot6ImagePath { get; set; }
            public string slot6Description { get; set; }
            public string slot7ImagePath { get; set; }
            public string slot7Description { get; set; }
            public string slot8ImagePath { get; set; }
            public string slot8Description { get; set; }
            public string slot9ImagePath { get; set; }
            public string slot9Description { get; set; }
            public string slot10ImagePath { get; set; }
            public string slot10Description { get; set; }

            public DateTime? uploadDate { get; set; }
            public bool? isActive { get; set; }
        }
    }
}