using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

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
      
        #region Car Variant CRUD

        [HttpPost]
        public IActionResult AddOrEditCarVariant(CarVariants variant)
        {
            try
            {
                bool result;
                if (variant.variantId == 0)
                {
                    variant.variantCreatedDate = DateTime.Now;
                    result = _dbConn.AddCarVariant(variant);
                }
                else
                {
                    result = _dbConn.UpdateCarVariant(variant);
                }
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetCarVariants(string? search)
        {
            try
            {
                var variants = _dbConn.GetCarVariants(search);
                return Json(variants);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetCarVariantById(int id)
        {
            try
            {
                var variant = _dbConn.GetCarVariantById(id);
                if (variant == null)
                    return Json(new { success = false, error = "Variant not found" });

                return Json(variant);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteCarVariant(int id)
        {
            try
            {
                var deleted = _dbConn.DeleteCarVariant(id);
                return Json(new { success = deleted });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        [HttpGet]
        public IActionResult GetCarModelsByBrand(int brandId)
        {
            try
            {
                var models = _dbConn.GetCarModelsByBrand(brandId);
                return Json(models);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        #endregion

        #region VehicleCategories CRUD

        [HttpGet]
        public IActionResult GetVehicleCategories(string? search)
        {
            try
            {
                var list = _dbConn.GetVehicleCategories(search);
                return Json(list);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetVehicleCategoryById(int id)
        {
            try
            {
                var category = _dbConn.GetVehicleCategoryById(id);
                if (category == null)
                    return Json(new { success = false, error = "Category not found" });
                return Json(category);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddOrEditVehicleCategory(VehicleCategories category)
        {
            try
            {
                bool result;
                if (category.categoryId == 0)
                    result = _dbConn.AddVehicleCategory(category);
                else
                    result = _dbConn.UpdateVehicleCategory(category);

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteVehicleCategory(int id)
        {
            try
            {
                var result = _dbConn.DeleteVehicleCategory(id);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        #endregion
        [HttpGet]
        public IActionResult GetFuelTypes(string? search)
        {
            try
            {
                var list = _dbConn.GetFuelTypes(search);
                return Json(list);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetFuelTypeById(int id)
        {
            try
            {
                var item = _dbConn.GetFuelTypeById(id);
                return Json(item);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddOrEditFuelType(FuelTypes fuelType)
        {
            try
            {
                if (fuelType.fuelTypeId == 0)
                {
                    var added = _dbConn.AddFuelType(fuelType);
                    return Json(added);
                }
                else
                {
                    var updated = _dbConn.UpdateFuelType(fuelType);
                    return Json(updated);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult DeleteFuelType(int id)
        {
            try
            {
                var deleted = _dbConn.DeleteFuelType(id);
                return Json(deleted);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult GetTransmissionTypes(string? search)
        {
            try
            {
                var result = _dbConn.GetTransmissionTypes(search);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public IActionResult GetTransmissionTypeById(int id)
        {
            try
            {
                var transmission = _dbConn.GetTransmissionTypeById(id);
                return Json(transmission);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddOrEditTransmissionType(TransmissionTypes transmission)
        {
            try
            {
                if (transmission.transmissionId == 0)
                {
                    var added = _dbConn.AddTransmissionType(transmission);
                    return Json(added);
                }
                else
                {
                    var updated = _dbConn.UpdateTransmissionType(transmission);
                    return Json(updated);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult DeleteTransmissionType(int id)
        {
            try
            {
                var deleted = _dbConn.DeleteTransmissionType(id);
                return Json(deleted);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

    }
}
