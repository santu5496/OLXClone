using DbOperation.Interface;
using DbOperation.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using static DbOperation.Implementation.ImageServices;

namespace Assignment4.Controllers
{
    public class ImagesController : Controller
    {
        private readonly IImageServices _imageService;

        public ImagesController(IImageServices imageService)
        {
            _imageService = imageService;
        }

        // View Page
        public IActionResult Images()
        {
            return View();
        }

        #region CRUD Operations

        /// <summary>
        /// Get all car images for listing in DataTable
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCarImages()
        {
            try
            {
                var images = await _imageService.GetAllImagesAsync();
                return Json(images);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Get specific car image by ID for editing
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCarImageById(int id)
        {
            try
            {
                var image = await _imageService.GetImageForEditAsync(id);
                if (image == null)
                    return Json(new { success = false, error = "Image not found" });

                return Json(image);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Add or Update car images - handles both create and update operations
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddOrUpdateCarImages1([FromBody] CarImageDto imageData)
        {
            try
            {
                // Validation
                if (imageData == null)
                {
                    return Json(new { success = false, message = "No image data provided" });
                }

                if (imageData.carRegistrationId <= 0)
                {
                    return Json(new { success = false, message = "Please select a valid car registration" });
                }

                // Check if at least one image is provided
                bool hasAnyImage = !string.IsNullOrEmpty(imageData.slot1ImageData) ||
                                   !string.IsNullOrEmpty(imageData.slot2ImageData) ||
                                   !string.IsNullOrEmpty(imageData.slot3ImageData) ||
                                   !string.IsNullOrEmpty(imageData.slot4ImageData) ||
                                   !string.IsNullOrEmpty(imageData.slot5ImageData) ||
                                   !string.IsNullOrEmpty(imageData.slot6ImageData) ||
                                   !string.IsNullOrEmpty(imageData.slot7ImageData) ||
                                   !string.IsNullOrEmpty(imageData.slot8ImageData) ||
                                   !string.IsNullOrEmpty(imageData.slot9ImageData) ||
                                   !string.IsNullOrEmpty(imageData.slot10ImageData);

                if (!hasAnyImage)
                {
                    return Json(new { success = false, message = "Please upload at least one image" });
                }

                // Perform add or update
                var result = await _imageService.AddOrUpdateImageAsync(imageData);

                if (result)
                {
                    var action = imageData.carImageId == 0 ? "added" : "updated";
                    return Json(new
                    {
                        success = true,
                        message = $"Car images {action} successfully"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to save car images. Please try again."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while saving images",
                    error = ex.Message
                });
            }
        }
        // Updated Controller Method with Enhanced Error Handling and Debugging
        [HttpPost]
        public async Task<IActionResult> AddOrUpdateCarImages([FromBody] CarImageDto imageData)
        {
            try
            {
                // STEP 1: Log incoming data for debugging
                Console.WriteLine($"Received request - CarImageId: {imageData?.carImageId}, CarRegistrationId: {imageData?.carRegistrationId}");

                // STEP 2: Basic validation
                if (imageData == null)
                {
                    Console.WriteLine("Error: No image data provided in request body");
                    return Json(new { success = false, message = "No image data provided" });
                }

                if (imageData.carRegistrationId <= 0)
                {
                    Console.WriteLine($"Error: Invalid carRegistrationId: {imageData.carRegistrationId}");
                    return Json(new { success = false, message = "Please select a valid car registration" });
                }

                if (string.IsNullOrEmpty(imageData.registrationNumber))
                {
                    Console.WriteLine("Error: registrationNumber is missing");
                    return Json(new { success = false, message = "Registration number is required" });
                }

                // STEP 3: Check if at least one image is provided
                bool hasAnyImage = !string.IsNullOrEmpty(imageData.slot1ImageData) ||
                                   !string.IsNullOrEmpty(imageData.slot2ImageData) ||
                                   !string.IsNullOrEmpty(imageData.slot3ImageData) ||
                                   !string.IsNullOrEmpty(imageData.slot4ImageData) ||
                                   !string.IsNullOrEmpty(imageData.slot5ImageData) ||
                                   !string.IsNullOrEmpty(imageData.slot6ImageData) ||
                                   !string.IsNullOrEmpty(imageData.slot7ImageData) ||
                                   !string.IsNullOrEmpty(imageData.slot8ImageData) ||
                                   !string.IsNullOrEmpty(imageData.slot9ImageData) ||
                                   !string.IsNullOrEmpty(imageData.slot10ImageData);

                if (!hasAnyImage)
                {
                    Console.WriteLine("Error: No images provided");
                    return Json(new { success = false, message = "Please upload at least one image" });
                }

                //// STEP 4: Validate constraints before attempting to save
                //var validationResult = await _imageService.ValidateCarImageConstraints(imageData);
                //if (!validationResult)
                //{
                //    Console.WriteLine("Validation failed before save attempt");
                //    return Json(new { success = false, message = "Validation failed. Check if the selected car exists." });
                //}

                // STEP 5: Log database schema info for debugging (remove in production)
               // var schemaInfo = await _imageService.GetDatabaseSchemaInfo();
               // Console.WriteLine($"Database Schema Info:\n{schemaInfo}");

                // STEP 6: Attempt to save
                Console.WriteLine("Attempting to save car images...");
                var result = await _imageService.AddOrUpdateImageAsync(imageData);

                if (result)
                {
                    var action = imageData.carImageId == 0 ? "added" : "updated";
                    Console.WriteLine($"Success: Car images {action} successfully");
                    return Json(new
                    {
                        success = true,
                        message = $"Car images {action} successfully",
                        carImageId = imageData.carImageId
                    });
                }
                else
                {
                    Console.WriteLine("Failed to save car images - service returned false");
                    return Json(new
                    {
                        success = false,
                        message = "Failed to save car images. Check console logs for detailed error information."
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Controller Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                return Json(new
                {
                    success = false,
                    message = "An error occurred while saving images",
                    error = ex.Message,
                    details = "Check server console logs for detailed error information"
                });
            }
        }

        // Additional debugging endpoint to test database connection and schema

        /// <summary>
        /// Delete car image record
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteCarImage(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new { success = false, message = "Invalid image ID" });
                }

                var result = await _imageService.DeleteImageAsync(id);

                if (result)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Car images deleted successfully"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Image record not found or already deleted"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while deleting the image",
                    error = ex.Message
                });
            }
        }

        #endregion

        #region Additional Utility Actions

        /// <summary>
        /// Check if a car already has images
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> HasCarImages(int listingId)
        {
            try
            {
                var hasImages = await _imageService.HasImagesForCarAsync(listingId);
                return Json(new { hasImages = hasImages });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Get images for a specific car registration
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetImagesByCarRegistration(int listingId)
        {
            try
            {
                var image = await _imageService.GetImageByCarRegistrationAsync(listingId);
                if (image == null)
                    return Json(new { success = false, message = "No images found for this car" });

                return Json(image);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        #endregion

        #region File Upload (Alternative method for direct file uploads)

        /// <summary>
        /// Alternative method for direct file upload (if needed)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UploadImageFile(IFormFile imageFile, int listingId, string registrationNumber, string description, int slotNumber = 1)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                    return Json(new { success = false, error = "No file uploaded" });

                if (slotNumber < 1 || slotNumber > 10)
                    return Json(new { success = false, error = "Invalid slot number. Must be between 1 and 10." });

                // Convert file to base64
                using var memoryStream = new System.IO.MemoryStream();
                await imageFile.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                var base64String = Convert.ToBase64String(imageBytes);

                // Create DTO with the uploaded image in the specified slot
                var imageDto = new CarImageDto
                {
                    carImageId = 0, // New record
                    carRegistrationId = listingId,
                    registrationNumber = registrationNumber,
                    isActive = true
                };

                // Set the image data for the specified slot
                var slotProperty = typeof(CarImageDto).GetProperty($"slot{slotNumber}ImageData");
                var descProperty = typeof(CarImageDto).GetProperty($"slot{slotNumber}Description");

                slotProperty?.SetValue(imageDto, base64String);
                descProperty?.SetValue(imageDto, description);

                var result = await _imageService.AddOrUpdateImageAsync(imageDto);

                if (result)
                {
                    return Json(new { success = true, message = "Image uploaded successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to save uploaded image" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        #endregion
    }
}