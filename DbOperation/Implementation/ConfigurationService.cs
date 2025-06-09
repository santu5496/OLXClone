using System;
using System.Collections.Generic;
using System.Linq;
using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.EntityFrameworkCore;

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

        public List<CarModels> GetCarModels(string? search = null)
        {
            using var Db = new Assignment4Context(_dbConn);
            var query = Db.CarModels.Include(m => m.brand).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(m => m.modelName.ToLower().Contains(lowerSearch)
                                      || m.brand.brandName.ToLower().Contains(lowerSearch));
            }

            return query.OrderBy(m => m.modelName).ToList();
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
    }
}
