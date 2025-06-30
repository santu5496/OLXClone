using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Assignment4.Controllers
{
    public class ImagesController : Controller
    {
        private readonly IImageServices _imageService;

        public ImagesController(IImageServices imageService)
        {
            _imageService = imageService;
        }

        // View Page (optional)
        public IActionResult Images()
        {
            return View();
        }

        #region Image CRUD

        [HttpGet]
        public async Task<IActionResult> GetAllImages()
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

        [HttpGet]
        public async Task<IActionResult> GetImageById(int id)
        {
            try
            {
                var image = await _imageService.GetImageByIdAsync(id);
                if (image == null)
                    return Json(new { success = false, error = "Image not found" });

                return Json(image);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateCarImages(CarImages image)
        {




            try
            {
                if (image.carImageId == 0 )
                {

                    await _imageService.AddImageAsync(image);
                    return Json(new { success = true });
                }
                else
                {
                    await _imageService.UpdateImageAsync(image);
                    return Json(new { success = true });

                }
            }


            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateImage(CarImages image)
        {
            try
            {
                await _imageService.UpdateImageAsync(image);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {
            try
            {
                await _imageService.DeleteImageAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        #endregion

        #region Upload Image From File

        [HttpPost]
        public async Task<IActionResult> UploadImageFromFile(IFormFile imageFile, int listingId, string registrationNumber, string description)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                    return Json(new { success = false, error = "Invalid image file" });

                using var stream = imageFile.OpenReadStream();
                await _imageService.AddImageFromStreamAsync(listingId, registrationNumber, stream, description);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        #endregion
    }
}
