using DbOperation.Interface;
using DbOperation.Models;
using DbOperation.ViewModels;
using Microsoft.EntityFrameworkCore;
using DbOperation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DbOperation.Implementation
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly DbContextOptions<Assignment4Context> _dbConn;

        public ConfigurationService(string dbConn)
        {
            _dbConn = new DbContextOptionsBuilder<Assignment4Context>()
                        .UseSqlServer(dbConn)
                        .Options;
        }
        

        // ======= Car Brands =======

        public List<CarBrands> GetCarBrands(string? search)
        {
            using var Db = new Assignment4Context(_dbConn);
            var query = Db.CarBrands.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(b => b.brandName.ToLower().Contains(lowerSearch));
            }

            return query.OrderBy(b => b.brandName).ToList();
        }

        public CarBrands? GetCarBrandById(int id)
        {
            using var Db = new Assignment4Context(_dbConn);
            return Db.CarBrands.Find(id);
        }

        public bool AddCarBrand(CarBrands brand)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var exists = Db.CarBrands.Any(b => b.brandName.ToLower() == brand.brandName.ToLower());
                if (exists) return false;

                Db.CarBrands.Add(brand);
                Db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateCarBrand(CarBrands brand)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var existing = Db.CarBrands.Find(brand.brandId);
                if (existing == null) return false;

                var exists = Db.CarBrands.Any(b => b.brandId != brand.brandId && b.brandName.ToLower() == brand.brandName.ToLower());
                if (exists) return false;

                existing.brandName = brand.brandName;
                existing.brandCountryOfOrigin = brand.brandCountryOfOrigin;
                existing.brandWebsite = brand.brandWebsite;
                existing.isLuxuryBrand = brand.isLuxuryBrand;
                existing.isActiveBrand = brand.isActiveBrand;
                existing.brandNotes = brand.brandNotes;

                Db.CarBrands.Update(existing);
                Db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteCarBrand(int id)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var brand = Db.CarBrands.Find(id);
                if (brand == null) return false;

                Db.CarBrands.Remove(brand);
                Db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ======= Car Models =======
        public List<CarModelNamveVewModel> GetCarModels(string? search = null)
        {
            using var Db = new Assignment4Context(_dbConn);

            var query = Db.CarModels.Include(m => m.brand).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(m =>
                    (m.modelName != null && m.modelName.ToLower().Contains(lowerSearch)) ||
                    (m.brand != null && m.brand.brandName != null && m.brand.brandName.ToLower().Contains(lowerSearch)));
            }

            var result = query.OrderBy(m => m.modelName)
                              .Select(m => new CarModelNamveVewModel
                              {
                                  brandId = m.brandId,
                                  brandName = m.brand != null ? m.brand.brandName : null,
                                  brandCountryOfOrigin = m.brand != null ? m.brand.brandCountryOfOrigin : null,
                                  brandWebsite = m.brand != null ? m.brand.brandWebsite : null,
                                  isLuxuryBrand = m.brand != null && m.brand.isLuxuryBrand.GetValueOrDefault(false),
                                  isActiveBrand = m.brand != null && m.brand.isActiveBrand.GetValueOrDefault(false),
                                  brandNotes = m.brand != null ? m.brand.brandNotes : null,
                                  modelName = m.modelName,
                                  modelLaunchYear = m.modelLaunchYear,
                                  modelGeneration = m.modelGeneration,
                                  modelId = m.modelId,
                                  modelBodyType = m.modelBodyType,
                                  modelDiscontinuedYear = m.modelDiscontinuedYear

                              }).ToList();

            return result;
        }



        // TransmissionTypes CRUD

        public List<TransmissionTypes> GetTransmissionTypes(string? search = null)
        {
            using var Db = new Assignment4Context(_dbConn);
            var query = Db.TransmissionTypes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(t =>
                    t.transmissionName.ToLower().Contains(lowerSearch) ||
                    (t.transmissionDescription != null && t.transmissionDescription.ToLower().Contains(lowerSearch))
                );
            }

            return query.OrderBy(t => t.transmissionName).ToList();
        }

        public TransmissionTypes GetTransmissionTypeById(int id)
        {
            using var Db = new Assignment4Context(_dbConn);
            return Db.TransmissionTypes.FirstOrDefault(t => t.transmissionId == id);
        }

        public bool AddTransmissionType(TransmissionTypes transmission)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                Db.TransmissionTypes.Add(transmission);
                Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateTransmissionType(TransmissionTypes transmission)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var existing = Db.TransmissionTypes.Find(transmission.transmissionId);
                if (existing == null) return false;

                existing.transmissionName = transmission.transmissionName;
                existing.transmissionFullName = transmission.transmissionFullName;
                existing.transmissionDescription = transmission.transmissionDescription;
                existing.easeOfDriving = transmission.easeOfDriving;
                existing.fuelEfficiencyImpact = transmission.fuelEfficiencyImpact;
                existing.maintenanceCost = transmission.maintenanceCost;
                existing.isActiveTransmission = transmission.isActiveTransmission;

                Db.TransmissionTypes.Update(existing);
                Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteTransmissionType(int id)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var transmission = Db.TransmissionTypes.Find(id);
                if (transmission == null) return false;

                Db.TransmissionTypes.Remove(transmission);
                Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }





        public CarModels? GetCarModelById(int id)
        {
            using var Db = new Assignment4Context(_dbConn);
            return Db.CarModels.Include(m => m.brand).FirstOrDefault(m => m.modelId == id);
        }

        public bool AddCarModel(CarModels model)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                // Prevent duplicate model names under same brand (case insensitive)
                var exists = Db.CarModels.Any(m => m.brandId == model.brandId && m.modelName.ToLower() == model.modelName.ToLower());
                if (exists) return false;

                Db.CarModels.Add(model);
                Db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateCarModel(CarModels model)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var existing = Db.CarModels.Find(model.modelId);
                if (existing == null) return false;

                // Check duplicate under same brand, excluding self
                var exists = Db.CarModels.Any(m => m.modelId != model.modelId && m.brandId == model.brandId && m.modelName.ToLower() == model.modelName.ToLower());
                if (exists) return false;

                existing.brandId = model.brandId;
                existing.modelName = model.modelName;
                existing.modelLaunchYear = model.modelLaunchYear;
                existing.modelDiscontinuedYear = model.modelDiscontinuedYear;
                existing.modelGeneration = model.modelGeneration;
                existing.modelBodyType = model.modelBodyType;
                existing.isActiveModel = model.isActiveModel;
                existing.modelDescription = model.modelDescription;

                Db.CarModels.Update(existing);
                Db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteCarModel(int id)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var model = Db.CarModels.Find(id);
                if (model == null) return false;

                Db.CarModels.Remove(model);
                Db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        // ====== CAR VARIANTS CRUD ======

        public bool AddCarVariant(CarVariants variant)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                variant.variantCreatedDate = DateTime.Now;
                Db.CarVariants.Add(variant);
                Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<CarVariantViewModel> GetCarVariants(string? search = null)
        {
            using var Db = new Assignment4Context(_dbConn);
            var query = Db.CarVariants
                          .Include(v => v.model)
                          .ThenInclude(m => m.brand)
                          .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(v =>
                    (v.variantName != null && v.variantName.ToLower().Contains(lowerSearch)) ||
                    (v.model != null && v.model.modelName != null && v.model.modelName.ToLower().Contains(lowerSearch)) ||
                    (v.model != null && v.model.brand != null && v.model.brand.brandName != null && v.model.brand.brandName.ToLower().Contains(lowerSearch))
                );
            }

            return query.OrderBy(v => v.variantName)
                        .Select(v => new CarVariantViewModel
                        {
                            variantId = v.variantId,
                            variantName = v.variantName,
                            variantTrimLevel = v.variantTrimLevel,
                            variantLaunchPrice = v.variantLaunchPrice,
                            variantCurrentPrice = v.variantCurrentPrice,
                            isActiveVariant = v.isActiveVariant,
                            variantCreatedDate = v.variantCreatedDate,
                            variantFeatures = v.variantFeatures,

                            modelId = v.modelId,
                            modelName = v.model != null ? v.model.modelName : null,

                            brandId = v.model != null && v.model.brand != null ? v.model.brand.brandId : 0,
                            brandName = v.model != null && v.model.brand != null ? v.model.brand.brandName : null,
                        })
                        .ToList();
        }


        public CarVariants GetCarVariantById(int id)
        {
            using var Db = new Assignment4Context(_dbConn);
            return Db.CarVariants.Include(v => v.model).FirstOrDefault(v => v.variantId == id);
        }

        public bool UpdateCarVariant(CarVariants variant)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var existingVariant = Db.CarVariants.Find(variant.variantId);
                if (existingVariant == null) return false;

                existingVariant.modelId = variant.modelId;
                existingVariant.variantName = variant.variantName;
                existingVariant.variantTrimLevel = variant.variantTrimLevel;
                existingVariant.variantLaunchPrice = variant.variantLaunchPrice;
                existingVariant.variantCurrentPrice = variant.variantCurrentPrice;
                existingVariant.isActiveVariant = variant.isActiveVariant;
                existingVariant.variantFeatures = variant.variantFeatures;

                Db.CarVariants.Update(existingVariant);
                Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteCarVariant(int id)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var variant = Db.CarVariants.Find(id);
                if (variant == null) return false;

                Db.CarVariants.Remove(variant);
                Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public List<CarVariants> GetCarVariantsByModel(int modelId)
        {
            using var Db = new Assignment4Context(_dbConn);
            return Db.CarVariants
                     .Include(v => v.model)
                     .Where(v => v.modelId == modelId)
                     .OrderBy(v => v.variantName)
                     .ToList();
        }
        public List<CarModels> GetCarModelsByBrand(int brandId)
        {
            using var Db = new Assignment4Context(_dbConn);
            return Db.CarModels
                     .Where(m => m.brandId == brandId)
                     .OrderBy(m => m.modelName)
                     .ToList();
        }



        #region start VehicleCategories CRUD

        public List<VehicleCategories> GetVehicleCategories(string? search = null)
        {
            using var Db = new Assignment4Context(_dbConn);
            var query = Db.VehicleCategories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(c =>
                    c.categoryName.ToLower().Contains(lowerSearch) ||
                    (c.categoryDescription != null && c.categoryDescription.ToLower().Contains(lowerSearch))
                );
            }

            return query.OrderBy(c => c.categoryName).ToList();
        }

        public VehicleCategories GetVehicleCategoryById(int id)
        {
            using var Db = new Assignment4Context(_dbConn);
            return Db.VehicleCategories.FirstOrDefault(c => c.categoryId == id);
        }

        public bool AddVehicleCategory(VehicleCategories category)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                Db.VehicleCategories.Add(category);
                Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateVehicleCategory(VehicleCategories category)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var existing = Db.VehicleCategories.Find(category.categoryId);
                if (existing == null) return false;

                existing.categoryName = category.categoryName;
                existing.categoryDescription = category.categoryDescription;
                existing.typicalSeatingCapacity = category.typicalSeatingCapacity;
                existing.typicalPriceRange = category.typicalPriceRange;
                existing.popularityRank = category.popularityRank;
                existing.isActiveCategory = category.isActiveCategory;

                Db.VehicleCategories.Update(existing);
                Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteVehicleCategory(int id)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var existing = Db.VehicleCategories.Find(id);
                if (existing == null) return false;

                Db.VehicleCategories.Remove(existing);
                Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        // ======== FuelTypes CRUD ========

        public List<FuelTypes> GetFuelTypes(string? search = null)
        {
            using var Db = new Assignment4Context(_dbConn);
            var query = Db.FuelTypes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(f =>
                    f.fuelTypeName.ToLower().Contains(lowerSearch) ||
                    (f.fuelTypeDescription != null && f.fuelTypeDescription.ToLower().Contains(lowerSearch))
                );
            }

            return query.OrderBy(f => f.fuelTypeName).ToList();
        }

        public FuelTypes? GetFuelTypeById(int id)
        {
            using var Db = new Assignment4Context(_dbConn);
            return Db.FuelTypes.Find(id);
        }

        public bool AddFuelType(FuelTypes fuelType)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                // Prevent duplicate fuel type names (case-insensitive)
                var exists = Db.FuelTypes.Any(f => f.fuelTypeName.ToLower() == fuelType.fuelTypeName.ToLower());
                if (exists) return false;

                Db.FuelTypes.Add(fuelType);
                Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateFuelType(FuelTypes fuelType)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var existing = Db.FuelTypes.Find(fuelType.fuelTypeId);
                if (existing == null) return false;

                // Prevent duplicate fuel type names on update
                var exists = Db.FuelTypes.Any(f => f.fuelTypeId != fuelType.fuelTypeId &&
                                                  f.fuelTypeName.ToLower() == fuelType.fuelTypeName.ToLower());
                if (exists) return false;

                existing.fuelTypeName = fuelType.fuelTypeName;
                existing.fuelTypeDescription = fuelType.fuelTypeDescription;
                existing.isEcoFriendly = fuelType.isEcoFriendly;
                existing.typicalFuelPrice = fuelType.typicalFuelPrice;
                existing.fuelEfficiencyRating = fuelType.fuelEfficiencyRating;
                existing.maintenanceCostLevel = fuelType.maintenanceCostLevel;
                existing.availabilityLevel = fuelType.availabilityLevel;
                existing.isActiveFuelType = fuelType.isActiveFuelType;

                Db.FuelTypes.Update(existing);
                Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteFuelType(int id)
        {
            using var Db = new Assignment4Context(_dbConn);
            try
            {
                var fuelType = Db.FuelTypes.Find(id);
                if (fuelType == null) return false;

                Db.FuelTypes.Remove(fuelType);
                Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
    

