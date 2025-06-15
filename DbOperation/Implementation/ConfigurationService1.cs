using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbOperation.Implementation
{
    public class ConfigurationService1 : IConfigurationService1
    {
        private readonly DbContextOptions<Assignment4Context> _dbConn;

        public ConfigurationService1(string dbConn)
        {
            _dbConn = new DbContextOptionsBuilder<Assignment4Context>()
                .UseSqlServer(dbConn)
                .Options;
        }

        // Add Car Color
        public CarColors AddCarColor(CarColors color)
        {
            try
            {
                using var _context = new Assignment4Context(_dbConn);

                if (color.isActiveColor == null)
                    color.isActiveColor = true;

                _context.CarColors.Add(color);
                _context.SaveChanges();
                return color;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Update Car Color
        public bool UpdateCarColor(CarColors color)
        {
            try
            {
                using var _context = new Assignment4Context(_dbConn);

                var existing = _context.CarColors.FirstOrDefault(c => c.colorId == color.colorId);
                if (existing == null) return true;

                existing.colorName = color.colorName;
                existing.colorDisplayName = color.colorDisplayName;
                existing.colorHexCode = color.colorHexCode;
                existing.colorFamily = color.colorFamily;
                existing.isMetallicPaint = color.isMetallicPaint;
                existing.isPearlPaint = color.isPearlPaint;
                existing.isSolidPaint = color.isSolidPaint;
                existing.popularityRank = color.popularityRank;
                existing.resaleValueImpact = color.resaleValueImpact;
                existing.isActiveColor = color.isActiveColor;
                _context.CarColors.Update(color);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }

        // Delete Car Color
        public bool DeleteCarColor(int id)
        {
            try
            {
                using var _context = new Assignment4Context(_dbConn);

                var existing = _context.CarColors.FirstOrDefault(c => c.colorId == id);
                if (existing == null) return false;

                _context.CarColors.Remove(existing);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
     


        public List<CarColors> GetAllColours()
        {
            try
            {
                // Fixed variable declaration - removed asterisks
                using var context = new Assignment4Context(_dbConn);

                // Fixed context reference - use the correct variable name
                var colours = context.CarColors.OrderBy(c => c.colorName).ToList();
                return colours;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                // Consider using a logging framework like NLog, Serilog, etc.
                Console.WriteLine($"Error in GetAllColours: {ex.Message}");
                return new List<CarColors>(); // Return empty list instead of null
            }
        }

        // PHASE 1: CRITICAL MASTER DATA

        // 1. Car Condition Levels Service



        // Add Car Condition Level
        public bool AddCarCondition(CarConditionLevels condition)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                if (condition.isActiveCondition == null)
                    condition.isActiveCondition = true;
                context.CarConditionLevels.Add(condition);
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Update Car Condition Level
        public bool UpdateCarCondition(CarConditionLevels condition)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.CarConditionLevels.FirstOrDefault(c => c.conditionId == condition.conditionId);
                if (existing == null) return true;

                existing.conditionName = condition.conditionName;
                existing.conditionDescription = condition.conditionDescription;
                existing.conditionPercentage = condition.conditionPercentage;
                existing.expectedPriceReduction = condition.expectedPriceReduction;
                existing.typicalIssues = condition.typicalIssues;
                existing.recommendedFor = condition.recommendedFor;
                existing.conditionSortOrder = condition.conditionSortOrder;
                existing.isActiveCondition = condition.isActiveCondition;
                context.CarConditionLevels.Update(existing);
                context.SaveChanges();
               
                return true;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        // Delete Car Condition Level
        public bool DeleteCarCondition(int id)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.CarConditionLevels.FirstOrDefault(c => c.conditionId == id);
                if (existing == null) return false;

                context.CarConditionLevels.Remove(existing);
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Get All Car Condition Levels
        public List<CarConditionLevels> GetAllCarConditions()
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var conditions = context.CarConditionLevels
                    .Where(c => c.isActiveCondition == true)
                    .OrderBy(c => c.conditionSortOrder)
                    .ToList();
                return conditions;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Get Car Condition by ID
        public CarConditionLevels GetCarConditionById(int id)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                return context.CarConditionLevels.FirstOrDefault(c => c.conditionId == id);
            }
            catch (Exception)
            {
                return null;
            }
        }




        // Add Geographic State
        public bool AddState(GeographicStates state)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                if (state.isActiveState == null)
                    state.isActiveState = true;
                if (state.popularForCars == null)
                    state.popularForCars = true;

                context.GeographicStates.Add(state);
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }

        public bool UpdateState(GeographicStates state)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.GeographicStates.FirstOrDefault(s => s.stateId == state.stateId);
                if (existing == null) return true;

                // Update properties
                existing.stateName = state.stateName;
                existing.stateCode = state.stateCode;
                existing.stateRegion = state.stateRegion;
                existing.countryName = state.countryName;
                existing.statePinCodePrefix = state.statePinCodePrefix;
                existing.popularForCars = state.popularForCars;
                existing.isActiveState = state.isActiveState;
                context.GeographicStates.Update(existing);

                // Save changes — EF will track and update the existing entity
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }


        // Delete Geographic State
        public bool DeleteState(int id)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.GeographicStates.FirstOrDefault(s => s.stateId == id);
                if (existing == null) return false;

                // Check if state has cities
                var hasCities = context.GeographicCities.Any(c => c.stateId == id);
                if (hasCities) return false; // Cannot delete state with cities

                context.GeographicStates.Remove(existing);
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Get All States
        public List<GeographicStates> GetAllStates()
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var states = context.GeographicStates
                    .Where(s => s.isActiveState == true)
                    .OrderBy(s => s.stateName)
                    .ToList();
                return states;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Get State by ID
        public GeographicStates GetStateById(int id)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                return context.GeographicStates.FirstOrDefault(s => s.stateId == id);
            }
            catch (Exception)
            {
                return null;
            }
        }


        // 3. Geographic Cities Service


        // Add Geographic City
        public GeographicCities AddCity(GeographicCities city)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                if (city.isActiveCity == null)
                    city.isActiveCity = true;
                if (city.hasGoodCarMarket == null)
                    city.hasGoodCarMarket = true;

                context.GeographicCities.Add(city);
                context.SaveChanges();
                return city;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Update Geographic City
        public GeographicCities UpdateCity(GeographicCities city)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.GeographicCities.FirstOrDefault(c => c.cityId == city.cityId);
                if (existing == null) return null;

                existing.stateId = city.stateId;
                existing.cityName = city.cityName;
                existing.cityType = city.cityType;
                existing.cityPopulation = city.cityPopulation;
                existing.hasGoodCarMarket = city.hasGoodCarMarket;
                existing.typicalCarDemand = city.typicalCarDemand;
                existing.isActiveCity = city.isActiveCity;

                context.SaveChanges();
                return existing;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Delete Geographic City
        public bool DeleteCity(int id)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.GeographicCities.FirstOrDefault(c => c.cityId == id);
                if (existing == null) return false;

                context.GeographicCities.Remove(existing);
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        public class CityDto
        {
            public int cityId { get; set; }
            public string cityName { get; set; }
            public string stateName { get; set; }
            public bool? isActiveCity { get; set; }

           

            public int stateId { get; set; }

      

            public string cityType { get; set; }

            public int? cityPopulation { get; set; }

            public bool? hasGoodCarMarket { get; set; }

            public string typicalCarDemand { get; set; }

       
            public virtual ICollection<CarListings> CarListings { get; set; } = new List<CarListings>();

            public virtual ICollection<RTOCodes> RTOCodes { get; set; } = new List<RTOCodes>();

            public virtual GeographicStates state { get; set; }
        }


        // Get All Cities
        public List<CityDto> GetAllCities()
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);

                var cities = context.GeographicCities
                    .Include(c => c.state)
                    .OrderBy(c => c.cityName)
                    .Select(c => new CityDto
                    {
                        cityId = c.cityId,
                        cityName = c.cityName,
                        stateId = c.stateId,
                        stateName = c.state.stateName,
                        cityType = c.cityType,
                        cityPopulation = c.cityPopulation,
                        hasGoodCarMarket = c.hasGoodCarMarket,
                        typicalCarDemand = c.typicalCarDemand,
                        isActiveCity = c.isActiveCity
                    })
                    .ToList();

                return cities;
            }
            catch (Exception)
            {
                return new List<CityDto>();
            }
        }



        // Get Cities by State
        public List<GeographicCities> GetCitiesByState(int stateId)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var cities = context.GeographicCities
                    .Where(c => c.stateId == stateId && c.isActiveCity == true)
                    .OrderBy(c => c.cityName)
                    .ToList();
                return cities;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Get City by ID
        public GeographicCities GetCityById(int id)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                return context.GeographicCities
                    .Include(c => c.state)
                    .FirstOrDefault(c => c.cityId == id);
            }
            catch (Exception)
            {
                return null;
            }
        }


        // PHASE 2: SUPPORTING MASTER DATA

        // 4. Engine Specifications Service


        // Add Engine Specification
        public EngineSpecifications AddEngineSpec(EngineSpecifications engineSpec)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                if (engineSpec.isActiveEngineSpec == null)
                    engineSpec.isActiveEngineSpec = true;
                if (engineSpec.naturallyAspiratedEngine == null)
                    engineSpec.naturallyAspiratedEngine = true;

                context.EngineSpecifications.Add(engineSpec);
                context.SaveChanges();
                return engineSpec;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Update Engine Specification
        public EngineSpecifications UpdateEngineSpec(EngineSpecifications engineSpec)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.EngineSpecifications.FirstOrDefault(e => e.engineSpecId == engineSpec.engineSpecId);
                if (existing == null) return null;

                existing.engineType = engineSpec.engineType;
                existing.engineCylinders = engineSpec.engineCylinders;
                existing.engineValveConfiguration = engineSpec.engineValveConfiguration;
                existing.turbochargedEngine = engineSpec.turbochargedEngine;
                existing.superchargedEngine = engineSpec.superchargedEngine;
                existing.naturallyAspiratedEngine = engineSpec.naturallyAspiratedEngine;
                existing.hybridSystem = engineSpec.hybridSystem;
                existing.engineDescription = engineSpec.engineDescription;
                existing.performanceRating = engineSpec.performanceRating;
                existing.isActiveEngineSpec = engineSpec.isActiveEngineSpec;

                context.SaveChanges();
                return existing;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Delete Engine Specification
        public bool DeleteEngineSpec(int id)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.EngineSpecifications.FirstOrDefault(e => e.engineSpecId == id);
                if (existing == null) return false;

                context.EngineSpecifications.Remove(existing);
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Get All Engine Specifications
        public List<EngineSpecifications> GetAllEngineSpecs()
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var engineSpecs = context.EngineSpecifications
                    .Where(e => e.isActiveEngineSpec == true)
                    .OrderBy(e => e.engineType)
                    .ThenBy(e => e.engineCylinders)
                    .ToList();
                return engineSpecs;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Get Engine Specification by ID
        public EngineSpecifications GetEngineSpecById(int id)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                return context.EngineSpecifications.FirstOrDefault(e => e.engineSpecId == id);
            }
            catch (Exception)
            {
                return null;
            }
        }


        // 5. Car Features List Service


        // Add Car Feature
        public CarFeaturesList AddCarFeature(CarFeaturesList feature)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                if (feature.isActiveFeature == null)
                    feature.isActiveFeature = true;
                if (feature.affectsResaleValue == null)
                    feature.affectsResaleValue = false;
                if (feature.isStandardFeature == null)
                    feature.isStandardFeature = false;

                context.CarFeaturesList.Add(feature);
                context.SaveChanges();
                return feature;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Update Car Feature
        public CarFeaturesList UpdateCarFeature(CarFeaturesList feature)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.CarFeaturesList.FirstOrDefault(f => f.featureId == feature.featureId);
                if (existing == null) return null;

                existing.featureName = feature.featureName;
                existing.featureDisplayName = feature.featureDisplayName;
                existing.featureCategory = feature.featureCategory;
                existing.featureSubCategory = feature.featureSubCategory;
                existing.featureDescription = feature.featureDescription;
                existing.featureImportanceLevel = feature.featureImportanceLevel;
                existing.typicalFoundIn = feature.typicalFoundIn;
                existing.affectsResaleValue = feature.affectsResaleValue;
                existing.isStandardFeature = feature.isStandardFeature;
                existing.isActiveFeature = feature.isActiveFeature;
                existing.featureIcon = feature.featureIcon;

                context.SaveChanges();
                return existing;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Delete Car Feature
        public bool DeleteCarFeature(int id)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.CarFeaturesList.FirstOrDefault(f => f.featureId == id);
                if (existing == null) return false;

                context.CarFeaturesList.Remove(existing);
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Get All Car Features
        public List<CarFeaturesList> GetAllCarFeatures()
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var features = context.CarFeaturesList
                    .Where(f => f.isActiveFeature == true)
                    .OrderBy(f => f.featureCategory)
                    .ThenBy(f => f.featureName)
                    .ToList();
                return features;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Get Features by Category
        public List<CarFeaturesList> GetFeaturesByCategory(string category)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var features = context.CarFeaturesList
                    .Where(f => f.featureCategory == category && f.isActiveFeature == true)
                    .OrderBy(f => f.featureName)
                    .ToList();
                return features;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Get Car Feature by ID
        public CarFeaturesList GetCarFeatureById(int id)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                return context.CarFeaturesList.FirstOrDefault(f => f.featureId == id);
            }
            catch (Exception)
            {
                return null;
            }
        }


        // 6. Insurance Providers Service


        // Add Insurance Provider
        public InsuranceProviders AddInsuranceProvider(InsuranceProviders provider)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                if (provider.isActiveProvider == null)
                    provider.isActiveProvider = true;

                context.InsuranceProviders.Add(provider);
                context.SaveChanges();
                return provider;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Update Insurance Provider
        public InsuranceProviders UpdateInsuranceProvider(InsuranceProviders provider)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.InsuranceProviders.FirstOrDefault(p => p.insuranceProviderId == provider.insuranceProviderId);
                if (existing == null) return null;

                existing.providerName = provider.providerName;
                existing.providerType = provider.providerType;
                existing.providerRating = provider.providerRating;
                existing.claimSettlementRatio = provider.claimSettlementRatio;
                existing.customerServiceRating = provider.customerServiceRating;
                existing.isActiveProvider = provider.isActiveProvider;

                context.SaveChanges();
                return existing;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Delete Insurance Provider
        public bool DeleteInsuranceProvider(int id)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.InsuranceProviders.FirstOrDefault(p => p.insuranceProviderId == id);
                if (existing == null) return false;

                context.InsuranceProviders.Remove(existing);
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Get All Insurance Providers
        public List<InsuranceProviders> GetAllInsuranceProviders()
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var providers = context.InsuranceProviders
                    .Where(p => p.isActiveProvider == true)
                    .OrderBy(p => p.providerName)
                    .ToList();
                return providers;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Get Insurance Provider by ID
        public InsuranceProviders GetInsuranceProviderById(int id)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                return context.InsuranceProviders.FirstOrDefault(p => p.insuranceProviderId == id);
            }
            catch (Exception)
            {
                return null;
            }
        }


        // 7. RTO Codes Service

        // Add RTO Code
        public RTOCodes AddRTOCode(RTOCodes rtoCode)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                if (rtoCode.isActiveRTO == null)
                    rtoCode.isActiveRTO = true;

                context.RTOCodes.Add(rtoCode);
                context.SaveChanges();
                return rtoCode;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Update RTO Code
        public RTOCodes UpdateRTOCode(RTOCodes rtoCode)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.RTOCodes.FirstOrDefault(r => r.rtoId == rtoCode.rtoId);
                if (existing == null) return null;

                existing.rtoCode = rtoCode.rtoCode;
                existing.rtoName = rtoCode.rtoName;
                existing.stateId = rtoCode.stateId;
                existing.cityId = rtoCode.cityId;
                existing.rtoAddress = rtoCode.rtoAddress;
                existing.rtoContactNumber = rtoCode.rtoContactNumber;
                existing.isActiveRTO = rtoCode.isActiveRTO;

                context.SaveChanges();
                return existing;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Delete RTO Code
        public bool DeleteRTOCode(int id)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.RTOCodes.FirstOrDefault(r => r.rtoId == id);
                if (existing == null) return false;

                context.RTOCodes.Remove(existing);
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Get All RTO Codes
        public List<RTOCodes> GetAllRTOCodes()
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var rtoCodes = context.RTOCodes
                    .Include(r => r.state)
                    .Include(r => r.city)
                    .Where(r => r.isActiveRTO == true)
                    .OrderBy(r => r.rtoCode)
                    .ToList();
                return rtoCodes;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Get RTO Codes by State
        public List<RTOCodes> GetRTOCodesByState(int stateId)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var rtoCodes = context.RTOCodes
                    .Include(r => r.city)
                    .Where(r => r.stateId == stateId && r.isActiveRTO == true)
                    .OrderBy(r => r.rtoCode)
                    .ToList();
                return rtoCodes;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Get RTO Code by ID
        public RTOCodes GetRTOCodeById(int id)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                return context.RTOCodes
                    .Include(r => r.state)
                    .Include(r => r.city)
                    .FirstOrDefault(r => r.rtoId == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Get RTO Code by Code
        public RTOCodes GetRTOCodeByCode(string code)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                return context.RTOCodes
                    .Include(r => r.state)
                    .Include(r => r.city)
                    .FirstOrDefault(r => r.rtoCode == code && r.isActiveRTO == true);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}



