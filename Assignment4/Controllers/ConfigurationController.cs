using System;
using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment4.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly IConfigurationService _dbConn;

        public ConfigurationController(IConfigurationService conn)
        {
            _dbConn = conn;
        }

        // Return the main CarBrands view page (optional)
        public IActionResult Configuration()
        {
            return View();
        }

        // Add or Edit CarBrand
        public IActionResult AddOrEditCarBrand(CarBrands brand)
        {
            try
            {
                if (brand.brandId == 0)
                {
                    var addedBrand = _dbConn.AddCarBrand(brand);
                    return Json(addedBrand);
                }
                else
                {
                    var updatedBrand = _dbConn.UpdateCarBrand(brand);
                    return Json(updatedBrand);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        // Get all CarBrands (optionally with search, implement in service if needed)
        public IActionResult GetCarBrands(string? search)
        {
            try
            {
                var brands = _dbConn.GetCarBrands(search);
                return Json(brands);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        // Get single CarBrand by Id (optional helper)
        public IActionResult GetCarBrandById(int id)
        {
            try
            {
                var brand = _dbConn.GetCarBrandById(id);
                return Json(brand);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        // Delete CarBrand by Id
        public IActionResult DeleteCarBrand(int id)
        {
            try
            {
                var deleted = _dbConn.DeleteCarBrand(id);
                return Json(deleted);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }



        // Add or Edit Car Model
        [HttpPost]
        public IActionResult AddOrEditCarModel(CarModels model)
        {
            try
            {
                bool result;
                if (model.modelId == 0)
                {
                    result = _dbConn.AddCarModel(model);
                }
                else
                {
                    result = _dbConn.UpdateCarModel(model);
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // Get all C [HttpGet]
        public IActionResult GetCarModels(string? search)
        {
            try
            {
                var models = _dbConn.GetCarModels(search);
                return Json(models);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // Get single Car Model by Id
        [HttpGet]
        public IActionResult GetCarModelById(int id)
        {
            try
            {
                var model = _dbConn.GetCarModelById(id);
                if (model == null)
                    return Json(new { success = false, error = "Model not found" });
                return Json(model);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // Delete Car Model by Id
        [HttpPost]
        public IActionResult DeleteCarModel(int id)
        {
            try
            {
                var deleted = _dbConn.DeleteCarModel(id);
                return Json(deleted);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // Get all Car Brands for dropdown (optional helper method)
        [HttpGet]
        public IActionResult GetAllCarBrands()
        {
            try
            {
                var brands = _dbConn.GetCarBrands(null);
                return Json(brands);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }








    }
}
