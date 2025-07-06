// =====================================================
// Corrected SearchViewService Implementation
// =====================================================

using DbOperation.Interface;
using DbOperation.Models;
using DbOperation.ViewModels;
using DbOperation.ViewModels.DbOperation.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.Implementation
{
    public class SearchViewService : ISearchViewService
    {
        private readonly DbContextOptions<Assignment4Context> _dbConn;

        public SearchViewService(string dbConn)
        {
            _dbConn = new DbContextOptionsBuilder<Assignment4Context>()
                .UseSqlServer(dbConn)
                .Options;
        }

        public async Task<CustomerCarSearchResultDto> SearchCarsAsync(CustomerCarSearchDto request)
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                IQueryable<CarListingDto> query = from listing in db.CarListings
                                                  join brand in db.CarBrands on listing.brandId equals brand.brandId
                                                  join model in db.CarModels on listing.modelId equals model.modelId
                                                  join fuelType in db.FuelTypes on listing.fuelTypeId equals fuelType.fuelTypeId
                                                  join transmission in db.TransmissionTypes on listing.transmissionId equals transmission.transmissionId
                                                  join city in db.GeographicCities on listing.cityId equals city.cityId
                                                  join state in db.GeographicStates on listing.stateId equals state.stateId
                                                  join variant in db.CarVariants on listing.variantId equals variant.variantId into variantGroup
                                                  from variant in variantGroup.DefaultIfEmpty()
                                                  join category in db.VehicleCategories on listing.categoryId equals category.categoryId into categoryGroup
                                                  from category in categoryGroup.DefaultIfEmpty()
                                                  join condition in db.CarConditionLevels on listing.overallConditionId equals condition.conditionId into conditionGroup
                                                  from condition in conditionGroup.DefaultIfEmpty()
                                                  where listing.listingStatus == "Active"
                                                  select new CarListingDto
                                                  {
                                                      listingId = listing.listingId,
                                                      listingTitle = listing.listingTitle,
                                                      brandName = brand.brandName,
                                                      modelName = model.modelName,
                                                      variantName = variant != null ? variant.variantName : null,
                                                      manufacturingYear = listing.manufacturingYear,
                                                      fuelType = fuelType.fuelTypeName,
                                                      transmission = transmission.transmissionName,
                                                      kilometersOnOdometer = listing.kilometersOnOdometer,
                                                      totalPreviousOwners = listing.totalPreviousOwners,
                                                      cityName = city.cityName,
                                                      stateName = state.stateName,
                                                      areaOrLocality = listing.areaOrLocality,
                                                      sellingPriceAsked = listing.sellingPriceAsked,
                                                      originalPurchasePrice = listing.originalPurchasePrice,
                                                      currentMarketPrice = listing.currentMarketPrice,
                                                      isPriceNegotiable = listing.isPriceNegotiable,
                                                      exteriorConditionRating = listing.exteriorConditionRating,
                                                      interiorConditionRating = listing.interiorConditionRating,
                                                      engineConditionRating = listing.engineConditionRating,
                                                      overallCondition = condition != null ? condition.conditionName : null,
                                                      isVerifiedListing = listing.isVerifiedListing,
                                                      isFeaturedListing = listing.isFeaturedListing,
                                                      isUrgentSale = listing.isUrgentSale,
                                                      hasAccidentHistory = listing.hasAccidentHistory,
                                                      sellerName = listing.sellerName,
                                                      sellerType = listing.sellerType,
                                                      sellerPrimaryPhone = listing.sellerPrimaryPhone,
                                                      totalViews = listing.totalViews,
                                                      totalInquiries = listing.totalInquiries,
                                                      listingCreatedDate = (DateTime)listing.listingCreatedDate,
                                                      brandId = listing.brandId,
                                                      modelId = listing.modelId,
                                                      categoryId = listing.categoryId,
                                                      fuelTypeId = listing.fuelTypeId,
                                                      transmissionId = listing.transmissionId,
                                                      cityId = listing.cityId,
                                                      stateId = listing.stateId
                                                  };

                // Apply filters and sorting
                query = ApplyFilters(query, request);
                var totalCount = await query.CountAsync();
                query = ApplySorting(query, request);

                // Pagination
                var skip = (request.page - 1) * request.pageSize;
                var results = await query.Skip(skip).Take(request.pageSize).ToListAsync();

                // Convert to detailed DTOs
                var carDtos = new List<CustomerCarDto>();
                foreach (var car in results)
                {
                    var carDto = await MapToCustomerCarDto(car);
                    carDtos.Add(carDto);
                }

                var filterOptions = await GetFilterOptionsAsync(db, request);
                var totalPages = (int)Math.Ceiling((double)totalCount / request.pageSize);

                return new CustomerCarSearchResultDto
                {
                    cars = carDtos,
                    totalCount = totalCount,
                    currentPage = request.page,
                    totalPages = totalPages,
                    hasNextPage = request.page < totalPages,
                    hasPreviousPage = request.page > 1,
                    availableBrands = filterOptions.brands,
                    availableModels = filterOptions.models,
                    availableCategories = filterOptions.categories,
                    availableFuelTypes = filterOptions.fuelTypes,
                    availableTransmissions = filterOptions.transmissions,
                    availableCities = filterOptions.cities,
                    minPrice = filterOptions.minPrice,
                    maxPrice = filterOptions.maxPrice,
                    minYear = filterOptions.minYear,
                    maxYear = filterOptions.maxYear,
                    totalListings = await GetTotalActiveListingsAsync(),
                    verifiedListings = await GetTotalVerifiedListingsAsync(),
                    featuredListings = await GetTotalFeaturedListingsAsync()
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching cars: {ex.Message}", ex);
            }
        }




        private IQueryable<CarListingDto> ApplyFilters(IQueryable<CarListingDto> query, CustomerCarSearchDto request)
        {
            if (!string.IsNullOrEmpty(request.searchKeyword))
            {
                var keyword = request.searchKeyword.ToLower();
                query = query.Where(x => x.listingTitle.ToLower().Contains(keyword) ||
                                         x.brandName.ToLower().Contains(keyword) ||
                                         x.modelName.ToLower().Contains(keyword));
            }

            if (request.cityId.HasValue)
                query = query.Where(x => x.cityId == request.cityId.Value);

            if (request.stateId.HasValue)
                query = query.Where(x => x.stateId == request.stateId.Value);

            if (request.brandId.HasValue)
                query = query.Where(x => x.brandId == request.brandId.Value);

            if (request.modelId.HasValue)
                query = query.Where(x => x.modelId == request.modelId.Value);

            if (request.categoryId.HasValue)
                query = query.Where(x => x.categoryId == request.categoryId.Value);

            if (request.fuelTypeId.HasValue)
                query = query.Where(x => x.fuelTypeId == request.fuelTypeId.Value);

            if (request.transmissionId.HasValue)
                query = query.Where(x => x.transmissionId == request.transmissionId.Value);

            if (request.minPrice.HasValue)
                query = query.Where(x => x.sellingPriceAsked >= request.minPrice.Value);

            if (request.maxPrice.HasValue)
                query = query.Where(x => x.sellingPriceAsked <= request.maxPrice.Value);

            if (request.minYear.HasValue)
                query = query.Where(x => x.manufacturingYear >= request.minYear.Value);

            if (request.maxYear.HasValue)
                query = query.Where(x => x.manufacturingYear <= request.maxYear.Value);

            if (request.minKM.HasValue)
                query = query.Where(x => x.kilometersOnOdometer >= request.minKM.Value);

            if (request.maxKM.HasValue)
                query = query.Where(x => x.kilometersOnOdometer <= request.maxKM.Value);

            if (request.firstOwnerOnly == true)
                query = query.Where(x => x.totalPreviousOwners == 1);

            if (request.verifiedOnly == true)
                query = query.Where(x => x.isVerifiedListing == true);

            if (request.noAccident == true)
                query = query.Where(x => x.hasAccidentHistory == false);

            if (request.automaticOnly == true)
                query = query.Where(x => x.transmission.ToLower().Contains("automatic"));

            return query;
        }




        private IQueryable<CarListingDto> ApplySorting(IQueryable<CarListingDto> query, CustomerCarSearchDto request)
        {
            return request.sortBy?.ToLower() switch
            {
                "price-low" => query.OrderBy(x => x.sellingPriceAsked),
                "price-high" => query.OrderByDescending(x => x.sellingPriceAsked),
                "year-new" => query.OrderByDescending(x => x.manufacturingYear),
                "year-old" => query.OrderBy(x => x.manufacturingYear),
                "km-low" => query.OrderBy(x => x.kilometersOnOdometer),
                "km-high" => query.OrderByDescending(x => x.kilometersOnOdometer),
                "popular" => query.OrderByDescending(x => x.totalViews ?? 0),
                _ => query.OrderByDescending(x => x.listingCreatedDate)
            };
        }



        private async Task<CustomerCarDto> MapToCustomerCarDto(dynamic car)
        {
            var carDto = new CustomerCarDto
            {
                listingId = car.ListingId,
                title = car.ListingTitle,
                brandName = car.BrandName,
                modelName = car.ModelName,
                variantName = car.VariantName,
                year = car.ManufacturingYear,
                fuelType = car.FuelType,
                transmission = car.Transmission,
                kilometersOnOdometer = car.KilometersOnOdometer,
                totalPreviousOwners = car.TotalPreviousOwners,
                cityName = car.CityName,
                stateName = car.StateName,
                areaOrLocality = car.AreaOrLocality,
                sellingPrice = car.SellingPriceAsked,
                originalPrice = car.OriginalPurchasePrice,
                marketPrice = car.CurrentMarketPrice,
                isPriceNegotiable = car.IsPriceNegotiable ?? false,
                exteriorRating = car.ExteriorConditionRating,
                interiorRating = car.InteriorConditionRating,
                engineRating = car.EngineConditionRating,
                overallCondition = car.OverallCondition,
                isVerified = car.IsVerifiedListing ?? false,
                isFeatured = car.IsFeaturedListing ?? false,
                isUrgentSale = car.IsUrgentSale ?? false,
                hasAccidentHistory = car.HasAccidentHistory ?? false,
                sellerName = car.SellerName,
                sellerType = car.SellerType,
                sellerPhone = car.SellerPrimaryPhone,
                totalViews = car.TotalViews,
                totalInquiries = car.TotalInquiries,
                listingDate = car.ListingCreatedDate
            };

            // Get images
            var images = await GetCarImagesAsync(car.ListingId);
            carDto.primaryImage = images.FirstOrDefault();
            carDto.additionalImages = images.Skip(1).ToList();
            carDto.totalImages = images.Count;

            // Get key features
            carDto.keyFeatures = await GetKeyFeaturesAsync(car.ListingId);

            // Calculate EMI
            carDto.emiAmount = await CalculateEMIAsync(carDto.sellingPrice * 0.8m, 8.5m, 60);

            // Calculate savings
            if (carDto.originalPrice.HasValue)
                carDto.savingsAmount = carDto.originalPrice.Value - carDto.sellingPrice;

            // Generate badges
            carDto.badges = GenerateBadges(carDto);

            // Generate price label
            carDto.priceLabel = GeneratePriceLabel(carDto);

            return carDto;
        }

        public async Task<CustomerCarDetailDto> GetCarDetailAsync(int listingId)
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                var carQuery = from listing in db.CarListings
                               join brand in db.CarBrands on listing.brandId equals brand.brandId
                               join model in db.CarModels on listing.modelId equals model.modelId
                               join fuelType in db.FuelTypes on listing.fuelTypeId equals fuelType.fuelTypeId
                               join transmission in db.TransmissionTypes on listing.transmissionId equals transmission.transmissionId
                               join city in db.GeographicCities on listing.cityId equals city.cityId
                               join state in db.GeographicStates on listing.stateId equals state.stateId
                               join variant in db.CarVariants on listing.variantId equals variant.variantId into variantGroup
                               from variant in variantGroup.DefaultIfEmpty()
                               join color in db.CarColors on listing.colorId equals color.colorId into colorGroup
                               from color in colorGroup.DefaultIfEmpty()
                               join rto in db.RTOCodes on listing.rtoId equals rto.rtoId into rtoGroup
                               from rto in rtoGroup.DefaultIfEmpty()
                               where listing.listingId == listingId
                               select new
                               {
                                   car = listing, // renamed to avoid conflict
                                   brandName = brand.brandName,
                                   modelName = model.modelName,
                                   variantName = variant != null ? variant.variantName : null,
                                   fuelType = fuelType.fuelTypeName,
                                   transmission = transmission.transmissionName,
                                   cityName = city.cityName,
                                   stateName = state.stateName,
                                   colorName = color != null ? color.colorName : null,
                                   rtoCode = rto != null ? rto.rtoCode : null
                               };

                var carData = await carQuery.FirstOrDefaultAsync();
                if (carData == null) return null;

                var car = carData.car;

                var detailDto = new CustomerCarDetailDto
                {
                    listingId = car.listingId,
                    title = car.listingTitle,
                    brandName = carData.brandName,
                    modelName = carData.modelName,
                    variantName = carData.variantName,
                    year = car.manufacturingYear,
                    fuelType = carData.fuelType,
                    transmission = carData.transmission,
                    kilometersOnOdometer = car.kilometersOnOdometer,
                    totalPreviousOwners = car.totalPreviousOwners,
                    cityName = carData.cityName,
                    stateName = carData.stateName,
                    areaOrLocality = car.areaOrLocality,
                    sellingPrice = car.sellingPriceAsked,
                    originalPrice = car.originalPurchasePrice,
                    marketPrice = car.currentMarketPrice,
                    isPriceNegotiable = car.isPriceNegotiable ?? false,

                    detailedDescription = car.detailedDescription,
                    sellingReason = car.sellingReason,
                    specialHighlights = car.specialHighlights,
                    knownIssues = car.knownIssuesOrProblems,

                    engineCC = car.engineCapacityInCC,
                    engineBHP = car.enginePowerInBHP,
                    mileage = car.combinedMileageKMPL,
                    seatingCapacity = car.seatingCapacity,
                    bootSpace = car.bootSpaceInLiters,
                    color = carData.colorName,

                    registrationNumber = car.registrationNumber,
                    rtoCode = carData.rtoCode,
                    insuranceType = car.insuranceType,
                    insuranceExpiry = car.insuranceExpiryDate?.ToDateTime(TimeOnly.MinValue),
                    pucExpiry = car.pollutionCertificateExpiryDate?.ToDateTime(TimeOnly.MinValue),
                    lastServiceDate = car.lastServiceDate?.ToDateTime(TimeOnly.MinValue),

                    hasCompleteServiceHistory = car.hasCompleteServiceHistory ?? false,
                    servicedAtAuthorized = car.servicedAtAuthorizedCenter ?? false,
                    lastServiceKM = car.lastServiceKilometers,

                    availableForInspection = car.availableForPhysicalInspection ?? false,
                    availableForTestDrive = car.availableForTestDrive ?? false,
                    preferredContactMethod = car.preferredContactMethod,
                    contactDays = car.availableForContactDays,
                    contactHours = car.availableForContactHours,

                    sellerName = car.sellerName,
                    sellerType = car.sellerType,
                    sellerPhone = car.sellerPrimaryPhone,

                    isVerified = car.isVerifiedListing ?? false,
                    isFeatured = car.isFeaturedListing ?? false,
                    isUrgentSale = car.isUrgentSale ?? false,
                    hasAccidentHistory = car.hasAccidentHistory ?? false,

                    exteriorRating = car.exteriorConditionRating,
                    interiorRating = car.interiorConditionRating,
                    engineRating = car.engineConditionRating,

                    totalViews = car.totalViews,
                    totalInquiries = car.totalInquiries,
                    listingDate = car.listingCreatedDate
                };

                // Images
                var images = await GetCarImagesAsync(listingId);
                detailDto.primaryImage = images.FirstOrDefault();
                detailDto.additionalImages = images.Skip(1).ToList();
                detailDto.totalImages = images.Count;

                // Features, Similar Cars, EMI, etc.
                detailDto.featureGroups = await GetCarFeatureGroupsAsync(listingId);
                detailDto.similarCars = await GetSimilarCarsAsync(listingId, 4);
                detailDto.emiAmount = await CalculateEMIAsync(detailDto.sellingPrice * 0.8m, 8.5m, 60);
                if (detailDto.originalPrice.HasValue)
                    detailDto.savingsAmount = detailDto.originalPrice.Value - detailDto.sellingPrice;

                detailDto.badges = GenerateBadges(detailDto);
                detailDto.priceLabel = GeneratePriceLabel(detailDto);

                return detailDto;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting car detail: {ex.Message}", ex);
            }
        }


    



        public async Task<HomePageDataDto> GetHomePageDataAsync()
        {
            try
            {
                var homeData = new HomePageDataDto
                {
                    featuredCars = await GetFeaturedCarsAsync(8),
                    recentCars = await GetRecentCarsAsync(8),
                    popularBrands = await GetBrandsByPopularityAsync(),
                    popularCities = await GetCitiesByCarCountAsync(),
                    popularSearches = await GetPopularSearchesAsync(),
                    totalActiveCars = await GetTotalActiveListingsAsync(),
                    totalVerifiedCars = await GetTotalVerifiedListingsAsync(),
                    totalFeaturedCars = await GetTotalFeaturedListingsAsync(),
                    carsByCategory = await GetCarCountByCategoryAsync(),
                    carsByPriceRange = await GetCarCountByPriceRangeAsync()
                };

                return homeData;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting home page data: {ex.Message}", ex);
            }
        }
        public async Task<List<CustomerCarDto>> GetFeaturedCarsAsync(int count = 10)
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                var featuredCars = await (from l in db.CarListings
                                          join b in db.CarBrands on l.brandId equals b.brandId
                                          join m in db.CarModels on l.modelId equals m.modelId
                                          join f in db.FuelTypes on l.fuelTypeId equals f.fuelTypeId
                                          join t in db.TransmissionTypes on l.transmissionId equals t.transmissionId
                                          join c in db.GeographicCities on l.cityId equals c.cityId
                                          join s in db.GeographicStates on l.stateId equals s.stateId
                                          where l.listingStatus == "Active" && l.isFeaturedListing == true
                                          orderby l.listingCreatedDate descending
                                          select new
                                          {
                                              listingId = l.listingId,
                                              listingTitle = l.listingTitle,
                                              brandName = b.brandName,
                                              modelName = m.modelName,
                                              manufacturingYear = l.manufacturingYear,
                                              fuelType = f.fuelTypeName,
                                              transmission = t.transmissionName,
                                              kilometersOnOdometer = l.kilometersOnOdometer,
                                              totalPreviousOwners = l.totalPreviousOwners,
                                              cityName = c.cityName,
                                              stateName = s.stateName,
                                              areaOrLocality = l.areaOrLocality,
                                              sellingPriceAsked = l.sellingPriceAsked,
                                              originalPurchasePrice = l.originalPurchasePrice,
                                              isVerifiedListing = l.isVerifiedListing,
                                              isFeaturedListing = l.isFeaturedListing,
                                              hasAccidentHistory = l.hasAccidentHistory,
                                              sellerName = l.sellerName,
                                              sellerType = l.sellerType,
                                              sellerPrimaryPhone = l.sellerPrimaryPhone,
                                              totalViews = l.totalViews,
                                              listingCreatedDate = l.listingCreatedDate
                                          })
                                         .Take(count)
                                         .ToListAsync();

                var carDtos = new List<CustomerCarDto>();
                foreach (var car in featuredCars)
                {
                    var carDto = await MapToCustomerCarDto(car);
                    carDtos.Add(carDto);
                }

                return carDtos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting featured cars: {ex.Message}", ex);
            }
        }





        public async Task<List<CustomerCarDto>> GetRecentCarsAsync(int count = 10)
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                var recentCars = await (from l in db.CarListings
                                        join b in db.CarBrands on l.brandId equals b.brandId
                                        join m in db.CarModels on l.modelId equals m.modelId
                                        join f in db.FuelTypes on l.fuelTypeId equals f.fuelTypeId
                                        join t in db.TransmissionTypes on l.transmissionId equals t.transmissionId
                                        join c in db.GeographicCities on l.cityId equals c.cityId
                                        join s in db.GeographicStates on l.stateId equals s.stateId
                                        where l.listingStatus == "Active"
                                        orderby l.listingCreatedDate descending
                                        select new
                                        {
                                            listingId = l.listingId,
                                            listingTitle = l.listingTitle,
                                            brandName = b.brandName,
                                            modelName = m.modelName,
                                            manufacturingYear = l.manufacturingYear,
                                            fuelType = f.fuelTypeName,
                                            transmission = t.transmissionName,
                                            kilometersOnOdometer = l.kilometersOnOdometer,
                                            totalPreviousOwners = l.totalPreviousOwners,
                                            cityName = c.cityName,
                                            stateName = s.stateName,
                                            areaOrLocality = l.areaOrLocality,
                                            sellingPriceAsked = l.sellingPriceAsked,
                                            originalPurchasePrice = l.originalPurchasePrice,
                                            isVerifiedListing = l.isVerifiedListing,
                                            isFeaturedListing = l.isFeaturedListing,
                                            hasAccidentHistory = l.hasAccidentHistory,
                                            sellerName = l.sellerName,
                                            sellerType = l.sellerType,
                                            sellerPrimaryPhone = l.sellerPrimaryPhone,
                                            totalViews = l.totalViews,
                                            listingCreatedDate = l.listingCreatedDate
                                        })
                                       .Take(count)
                                       .ToListAsync();

                var carDtos = new List<CustomerCarDto>();
                foreach (var car in recentCars)
                {
                    var carDto = await MapToCustomerCarDto(car);
                    carDtos.Add(carDto);
                }

                return carDtos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting recent cars: {ex.Message}", ex);
            }
        }



        public async Task<List<CustomerCarDto>> GetSimilarCarsAsync(int listingId, int count = 5)
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                var referenceCar = await db.CarListings
                    .Where(x => x.listingId == listingId)
                    .Select(x => new
                    {
                        x.brandId,
                        x.modelId,
                        x.categoryId,
                        x.sellingPriceAsked,
                        x.manufacturingYear
                    })
                    .FirstOrDefaultAsync();

                if (referenceCar == null)
                    return new List<CustomerCarDto>();

                var priceRange = referenceCar.sellingPriceAsked * 0.3m;

                var similarCars = await (from l in db.CarListings
                                         join b in db.CarBrands on l.brandId equals b.brandId
                                         join m in db.CarModels on l.modelId equals m.modelId
                                         join f in db.FuelTypes on l.fuelTypeId equals f.fuelTypeId
                                         join t in db.TransmissionTypes on l.transmissionId equals t.transmissionId
                                         join c in db.GeographicCities on l.cityId equals c.cityId
                                         join s in db.GeographicStates on l.stateId equals s.stateId
                                         where l.listingStatus == "Active" &&
                                               l.listingId != listingId &&
                                               (l.brandId == referenceCar.brandId || l.categoryId == referenceCar.categoryId) &&
                                               l.sellingPriceAsked >= (referenceCar.sellingPriceAsked - priceRange) &&
                                               l.sellingPriceAsked <= (referenceCar.sellingPriceAsked + priceRange) &&
                                               Math.Abs(l.manufacturingYear - referenceCar.manufacturingYear) <= 3
                                         orderby l.totalViews descending
                                         select new
                                         {
                                             listingId = l.listingId,
                                             listingTitle = l.listingTitle,
                                             brandName = b.brandName,
                                             modelName = m.modelName,
                                             manufacturingYear = l.manufacturingYear,
                                             fuelType = f.fuelTypeName,
                                             transmission = t.transmissionName,
                                             kilometersOnOdometer = l.kilometersOnOdometer,
                                             totalPreviousOwners = l.totalPreviousOwners,
                                             cityName = c.cityName,
                                             stateName = s.stateName,
                                             areaOrLocality = l.areaOrLocality,
                                             sellingPriceAsked = l.sellingPriceAsked,
                                             originalPurchasePrice = l.originalPurchasePrice,
                                             isVerifiedListing = l.isVerifiedListing,
                                             isFeaturedListing = l.isFeaturedListing,
                                             hasAccidentHistory = l.hasAccidentHistory,
                                             sellerName = l.sellerName,
                                             sellerType = l.sellerType,
                                             sellerPrimaryPhone = l.sellerPrimaryPhone,
                                             totalViews = l.totalViews,
                                             listingCreatedDate = l.listingCreatedDate
                                         })
                                        .Take(count)
                                        .ToListAsync();

                var carDtos = new List<CustomerCarDto>();
                foreach (var car in similarCars)
                {
                    var carDto = await MapToCustomerCarDto(car);
                    carDtos.Add(carDto);
                }

                return carDtos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting similar cars: {ex.Message}", ex);
            }
        }
        public async Task<List<string>> GetCarImagesAsync(int listingId)
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                var images = new List<string>();

                var carImages = await db.CarImages
     .Where(x => x.listingId == listingId && x.isActive == true)
     .FirstOrDefaultAsync();


                if (carImages != null)
                {
                    var properties = typeof(CarImages).GetProperties(); // corrected class name

                    for (int i = 1; i <= 15; i++)
                    {
                        var prop = properties.FirstOrDefault(p => p.Name == $"Slot{i}ImageData");
                        var value = prop?.GetValue(carImages);
                        if (value != null)
                        {
                            images.Add($"/images/cars/{listingId}/{i}.jpg");
                        }
                    }
                }

                return images;
            }
            catch
            {
                return new List<string>();
            }
        }




        // Helper methods



        private async Task<List<string>> GetKeyFeaturesAsync(int listingId)
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                var features = await (from lf in db.CarListingFeatures
                                      join f in db.CarFeaturesList on lf.featureId equals f.featureId
                                      where lf.listingId == listingId && lf.isFeatureAvailable == true
                                      orderby f.featureImportanceLevel
                                      select f.featureDisplayName ?? f.featureName)
                                     .Take(4)
                                     .ToListAsync();

                return features;
            }
            catch
            {
                return new List<string>();
            }
        }


        private async Task<List<CarFeatureGroupDto>> GetCarFeatureGroupsAsync(int listingId)
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                var features = await (from lf in db.CarListingFeatures
                                      join f in db.CarFeaturesList on lf.featureId equals f.featureId
                                      where lf.listingId == listingId
                                      select new
                                      {
                                          CategoryName = f.featureCategory ?? "Other",
                                          FeatureName = f.featureDisplayName ?? f.featureName,
                                          IsAvailable = lf.isFeatureAvailable ?? false,
                                          Condition = lf.featureCondition
                                      })
                                    .ToListAsync();

                var featureGroups = features
                    .GroupBy(x => x.CategoryName)
                    .Select(g => new CarFeatureGroupDto
                    {
                        categoryName = g.Key,
                        features = g.Select(f => new CarFeatureItemDto
                        {
                            featureName = f.FeatureName,
                            isAvailable = f.IsAvailable,
                            condition = f.Condition
                        }).ToList()
                    })
                    .ToList();

                return featureGroups;
            }
            catch
            {
                return new List<CarFeatureGroupDto>();
            }
        }

        private List<string> GenerateBadges(CustomerCarDto car)
        {
            var badges = new List<string>();

            if (car.isVerified) badges.Add("Verified");
            if (car.isFeatured) badges.Add("Featured");
            if (car.isUrgentSale) badges.Add("Urgent");
            if (car.totalPreviousOwners == 1) badges.Add("First Owner");
            if (!car.hasAccidentHistory) badges.Add("No Accident");
            if (car.savingsAmount > 50000) badges.Add("Great Deal");

            return badges;
        }

        private string GeneratePriceLabel(CustomerCarDto car)
        {
            if (!car.marketPrice.HasValue) return "Fair Price";

            var priceDiff = car.marketPrice.Value - car.sellingPrice;
            var percentDiff = (priceDiff / car.marketPrice.Value) * 100;

            return percentDiff switch
            {
                > 15 => "Excellent Deal",
                > 8 => "Great Deal",
                > 3 => "Good Deal",
                > -3 => "Fair Price",
                _ => "Above Market"
            };
        }

        public async Task<List<FilterOption>> GetCitiesByCarCountAsync()
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                return await (from l in db.CarListings
                              join c in db.GeographicCities on l.cityId equals c.cityId
                              where l.listingStatus == "Active"
                              group l by new { c.cityId, c.cityName } into g
                              orderby g.Count() descending
                              select new FilterOption
                              {
                                  id = g.Key.cityId,
                                  name = g.Key.cityName,
                                  count = g.Count()
                              })
                             .Take(20)
                             .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting cities by car count: {ex.Message}", ex);
            }
        }
        public async Task<List<FilterOption>> GetModelsByBrandAsync(int brandId)
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                return await (from l in db.CarListings
                              join m in db.CarModels on l.modelId equals m.modelId
                              where l.listingStatus == "Active" && l.brandId == brandId
                              group l by new { m.modelId, m.modelName } into g
                              orderby g.Count() descending
                              select new FilterOption
                              {
                                  id = g.Key.modelId,
                                  name = g.Key.modelName,
                                  count = g.Count()
                              })
                             .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting models by brand: {ex.Message}", ex);
            }
        }

        public async Task<List<FilterOption>> GetCategoriesAsync()
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                return await (from l in db.CarListings
                              join c in db.VehicleCategories on l.categoryId equals c.categoryId into cg
                              from c in cg.DefaultIfEmpty()
                              where l.listingStatus == "Active" && c != null
                              group l by new { c.categoryId, c.categoryName } into g
                              orderby g.Count() descending
                              select new FilterOption
                              {
                                  id = g.Key.categoryId,
                                  name = g.Key.categoryName,
                                  count = g.Count()
                              })
                             .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting categories: {ex.Message}", ex);
            }
        }
        public async Task<List<FilterOption>> GetBrandsByPopularityAsync()
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                return await (from l in db.CarListings
                              join b in db.CarBrands on l.brandId equals b.brandId
                              where l.listingStatus == "Active"
                              group l by new { b.brandId, b.brandName } into g
                              orderby g.Count() descending
                              select new FilterOption
                              {
                                  id = g.Key.brandId,
                                  name = g.Key.brandName,
                                  count = g.Count()
                              })
                             .Take(20)
                             .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting brands by popularity: {ex.Message}", ex);
            }
        }

        public async Task<List<FilterOption>> GetFuelTypesAsync()
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                return await (from l in db.CarListings
                              join f in db.FuelTypes on l.fuelTypeId equals f.fuelTypeId
                              where l.listingStatus == "Active"
                              group l by new { f.fuelTypeId, f.fuelTypeName } into g
                              orderby g.Count() descending
                              select new FilterOption
                              {
                                  id = g.Key.fuelTypeId,
                                  name = g.Key.fuelTypeName,
                                  count = g.Count()
                              })
                             .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting fuel types: {ex.Message}", ex);
            }
        }
        public async Task<List<FilterOption>> GetTransmissionTypesAsync()
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                return await (from listing in db.CarListings
                              join transmission in db.TransmissionTypes on listing.transmissionId equals transmission.transmissionId
                              where listing.listingStatus == "Active"
                              group listing by new { transmission.transmissionId, transmission.transmissionName } into g
                              orderby g.Count() descending
                              select new FilterOption
                              {
                                  id = g.Key.transmissionId,
                                  name = g.Key.transmissionName,
                                  count = g.Count()
                              }).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting transmission types: {ex.Message}", ex);
            }
        }

        public async Task<bool> RecordCarViewAsync(int listingId, string ipAddress = null)
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                var listing = await db.CarListings.FindAsync(listingId);
                if (listing != null)
                {
                    listing.totalViews = (listing.totalViews ?? 0) + 1;
                }

                var carView = new CarListingViews
                {
                    listingId = listingId,
                    viewDate = DateTime.Now,
                    viewerIPAddress = ipAddress,
                    pagesViewed = 1,
                    imagesViewed = 0,
                    contactDetailsViewed = false,
                    phoneNumberRevealed = false
                };

                db.CarListingViews.Add(carView);
                await db.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SubmitInquiryAsync(CustomerInquiryDto inquiry)
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                var customerInquiry = new CustomerInquiries
                {
                    listingId = inquiry.listingId,
                    inquiryType = inquiry.inquiryType,
                    customerName = inquiry.customerName,
                    customerPhone = inquiry.customerPhone,
                    customerEmail = inquiry.customerEmail,
                    inquiryMessage = inquiry.message,
                    budgetRange = inquiry.budgetRange,
                    financeRequired = inquiry.financeRequired,
                    inquiryDate = DateTime.Now,
                    inquiryStatus = "New",
                    priorityLevel = "Medium",
                    followUpRequired = false,
                    viewedCarPhysically = false,
                    tookTestDrive = false,
                    madeOffer = false,
                    purchasedCar = false
                };

                db.CustomerInquiries.Add(customerInquiry);

                var listing = await db.CarListings.FindAsync(inquiry.listingId);
                if (listing != null)
                {
                    listing.totalInquiries = (listing.totalInquiries ?? 0) + 1;
                }

                await db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<decimal> CalculateEMIAsync(decimal amount, decimal rate, int months)
        {
            await Task.CompletedTask;

            if (amount <= 0 || rate <= 0 || months <= 0)
                return 0;

            var monthlyRate = rate / 100 / 12;
            var emi = amount * monthlyRate * (decimal)Math.Pow((double)(1 + monthlyRate), months) /
                     ((decimal)Math.Pow((double)(1 + monthlyRate), months) - 1);

            return Math.Round(emi, 0);
        }

        public async Task<int> GetTotalActiveListingsAsync()
        {
            using var db = new Assignment4Context(_dbConn);
            return await db.CarListings.Where(x => x.listingStatus == "Active").CountAsync();
        }

        public async Task<int> GetTotalVerifiedListingsAsync()
        {
            using var db = new Assignment4Context(_dbConn);
            return await db.CarListings.Where(x => x.listingStatus == "Active" && x.isVerifiedListing == true).CountAsync();
        }

        public async Task<int> GetTotalFeaturedListingsAsync()
        {
            using var db = new Assignment4Context(_dbConn);
            return await db.CarListings.Where(x => x.listingStatus == "Active" && x.isFeaturedListing == true).CountAsync();
        }
        public async Task<Dictionary<string, int>> GetCarCountByCategoryAsync()
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                return await (from listing in db.CarListings
                              join category in db.VehicleCategories on listing.categoryId equals category.categoryId into categoryGroup
                              from category in categoryGroup.DefaultIfEmpty()
                              where listing.listingStatus == "Active"
                              group listing by (category != null ? category.categoryName : "Other") into g
                              select new { category = g.Key, count = g.Count() })
                             .ToDictionaryAsync(x => x.category, x => x.count);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting car count by category: {ex.Message}", ex);
            }
        }

        public async Task<Dictionary<string, int>> GetCarCountByPriceRangeAsync()
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                var prices = await db.CarListings
                    .Where(x => x.listingStatus == "Active")
                    .Select(x => x.sellingPriceAsked)
                    .ToListAsync();

                var priceRanges = new Dictionary<string, int>
                {
                    ["Under ₹2L"] = prices.Count(x => x < 200000),
                    ["₹2L - ₹5L"] = prices.Count(x => x >= 200000 && x < 500000),
                    ["₹5L - ₹10L"] = prices.Count(x => x >= 500000 && x < 1000000),
                    ["₹10L - ₹20L"] = prices.Count(x => x >= 1000000 && x < 2000000),
                    ["Above ₹20L"] = prices.Count(x => x >= 2000000)
                };

                return priceRanges;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting car count by price range: {ex.Message}", ex);
            }
        }


        public async Task<Dictionary<string, int>> GetCarCountByLocationAsync()
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                return await (from listing in db.CarListings
                              join city in db.GeographicCities on listing.cityId equals city.cityId
                              where listing.listingStatus == "Active"
                              group listing by city.cityName into g
                              orderby g.Count() descending
                              select new { city = g.Key, count = g.Count() })
                             .Take(10)
                             .ToDictionaryAsync(x => x.city, x => x.count);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting car count by location: {ex.Message}", ex);
            }
        }


        // Additional interface methods implementation
        public async Task<List<CustomerCarDto>> GetCarsByBrandAsync(int brandId, int count = 20)
        {
            var request = new CustomerCarSearchDto { brandId = brandId, pageSize = count };
            var result = await SearchCarsAsync(request);
            return result.cars;
        }

        public async Task<List<CustomerCarDto>> GetCarsByPriceRangeAsync(decimal minPrice, decimal maxPrice, int count = 20)
        {
            var request = new CustomerCarSearchDto { minPrice = minPrice, maxPrice = maxPrice, pageSize = count };
            var result = await SearchCarsAsync(request);
            return result.cars;
        }

        public async Task<List<PopularSearchDto>> GetPopularSearchesAsync()
        {
            // This would typically come from search analytics table
            // For now, return some static popular searches
            return new List<PopularSearchDto>
            {
                new PopularSearchDto { searchTerm = "Swift", searchCount = 1250, category = "Model" },
                new PopularSearchDto { searchTerm = "Under 5 Lakh", searchCount = 980, category = "Price" },
                new PopularSearchDto { searchTerm = "Automatic", searchCount = 856, category = "Transmission" },
                new PopularSearchDto { searchTerm = "Creta", searchCount = 745, category = "Model" },
                new PopularSearchDto { searchTerm = "First Owner", searchCount = 689, category = "Ownership" }
            };
        }
        public async Task<FilterStatisticsDto> GetFilterStatisticsAsync(CustomerCarSearchDto request)
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                var query = db.CarListings.Where(x => x.listingStatus == "Active");

                // Apply filter
                if (request.cityId.HasValue)
                    query = query.Where(x => x.cityId == request.cityId.Value);

                var statistics = new FilterStatisticsDto();

                // Brand counts
                var brandCounts = await (from listing in query
                                         join brand in db.CarBrands on listing.brandId equals brand.brandId
                                         group listing by brand.brandName into g
                                         select new { brand = g.Key, count = g.Count() })
                                        .ToDictionaryAsync(x => x.brand, x => x.count);

                statistics.brandCounts = brandCounts;

                return statistics;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting filter statistics: {ex.Message}", ex);
            }
        }


        public async Task<bool> AddToFavoritesAsync(int listingId, string userId)
        {
            // Implementation depends on your user/favorites system
            await Task.CompletedTask;
            return true;
        }

        public async Task<bool> RemoveFromFavoritesAsync(int listingId, string userId)
        {
            // Implementation depends on your user/favorites system
            await Task.CompletedTask;
            return true;
        }

        public async Task<List<CustomerCarDto>> GetUserFavoritesAsync(string userId)
        {
            // Implementation depends on your user/favorites system
            await Task.CompletedTask;
            return new List<CustomerCarDto>();
        }

        public async Task<List<CarFeatureItemDto>> GetCarFeaturesAsync(int listingId)
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                return await (from featureLink in db.CarListingFeatures
                              join feature in db.CarFeaturesList on featureLink.featureId equals feature.featureId
                              where featureLink.listingId == listingId
                              select new CarFeatureItemDto
                              {
                                  featureName = feature.featureDisplayName ?? feature.featureName,
                                  isAvailable = featureLink.isFeatureAvailable ?? false,
                                  condition = featureLink.featureCondition
                              }).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting car features: {ex.Message}", ex);
            }
        }


        public async Task<List<CustomerCarDto>> QuickSearchAsync(string keyword, int? cityId = null, int count = 10)
        {
            var request = new CustomerCarSearchDto
            {
                searchKeyword = keyword,
                cityId = cityId,
                pageSize = count
            };
            var result = await SearchCarsAsync(request);
            return result.cars;
        }
        public async Task<List<string>> GetSearchSuggestionsAsync(string keyword)
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                if (string.IsNullOrWhiteSpace(keyword) || keyword.Length < 2)
                    return new List<string>();

                var suggestions = new List<string>();
                keyword = keyword.ToLower();

                // Brand suggestions
                var brandSuggestions = await db.CarBrands
                    .Where(b => b.brandName.ToLower().Contains(keyword))
                    .Select(b => b.brandName)
                    .Take(5)
                    .ToListAsync();
                suggestions.AddRange(brandSuggestions);

                // Model suggestions
                var modelSuggestions = await db.CarModels
                    .Where(m => m.modelName.ToLower().Contains(keyword))
                    .Select(m => m.modelName)
                    .Take(5)
                    .ToListAsync();
                suggestions.AddRange(modelSuggestions);

                // Category suggestions
                var categorySuggestions = await db.VehicleCategories
                    .Where(c => c.categoryName.ToLower().Contains(keyword))
                    .Select(c => c.categoryName)
                    .Take(3)
                    .ToListAsync();
                suggestions.AddRange(categorySuggestions);

                return suggestions
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct()
                    .Take(10)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting search suggestions: {ex.Message}", ex);
            }
        }


        public async Task<CustomerCarSearchResultDto> GetCarsByFiltersAsync(
            int? brandId = null,
            int? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? cityId = null,
            int page = 1,
            int pageSize = 20)
        {
            var request = new CustomerCarSearchDto
            {
                brandId = brandId,
                categoryId = categoryId,
                minPrice = minPrice,
                maxPrice = maxPrice,
                cityId = cityId,
                page = page,
                pageSize = pageSize
            };

            return await SearchCarsAsync(request);
        }


     private async Task<(
    List<FilterOption> brands,
    List<FilterOption> models,
    List<FilterOption> categories,
    List<FilterOption> fuelTypes,
    List<FilterOption> transmissions,
    List<FilterOption> cities,
    decimal minPrice,
    decimal maxPrice,
    int minYear,
    int maxYear)> GetFilterOptionsAsync(Assignment4Context db, CustomerCarSearchDto request)
        {
            var activeListings = db.CarListings.Where(x => x.listingStatus == "Active");

            if (request.cityId.HasValue)
                activeListings = activeListings.Where(x => x.cityId == request.cityId.Value);

            if (request.stateId.HasValue)
                activeListings = activeListings.Where(x => x.stateId == request.stateId.Value);

            var brands = await (from listing in activeListings
                                join brand in db.CarBrands on listing.brandId equals brand.brandId
                                group listing by new { brand.brandId, brand.brandName } into g
                                orderby g.Count() descending
                                select new FilterOption
                                {
                                    id = g.Key.brandId,
                                    name = g.Key.brandName,
                                    count = g.Count()
                                }).ToListAsync();

            var models = new List<FilterOption>();
            if (request.brandId.HasValue)
            {
                models = await (from listing in activeListings
                                join model in db.CarModels on listing.modelId equals model.modelId
                                where listing.brandId == request.brandId.Value
                                group listing by new { model.modelId, model.modelName } into g
                                orderby g.Count() descending
                                select new FilterOption
                                {
                                    id = g.Key.modelId,
                                    name = g.Key.modelName,
                                    count = g.Count()
                                }).ToListAsync();
            }

            var categories = await (from listing in activeListings
                                    join category in db.VehicleCategories on listing.categoryId equals category.categoryId into categoryGroup
                                    from category in categoryGroup.DefaultIfEmpty()
                                    where category != null
                                    group listing by new { category.categoryId, category.categoryName } into g
                                    orderby g.Count() descending
                                    select new FilterOption
                                    {
                                        id = g.Key.categoryId,
                                        name = g.Key.categoryName,
                                        count = g.Count()
                                    }).ToListAsync();

            var fuelTypes = await (from listing in activeListings
                                   join fuel in db.FuelTypes on listing.fuelTypeId equals fuel.fuelTypeId
                                   group listing by new { fuel.fuelTypeId, fuel.fuelTypeName } into g
                                   orderby g.Count() descending
                                   select new FilterOption
                                   {
                                       id = g.Key.fuelTypeId,
                                       name = g.Key.fuelTypeName,
                                       count = g.Count()
                                   }).ToListAsync();

            var transmissions = await (from listing in activeListings
                                       join transmission in db.TransmissionTypes on listing.transmissionId equals transmission.transmissionId
                                       group listing by new { transmission.transmissionId, transmission.transmissionName } into g
                                       orderby g.Count() descending
                                       select new FilterOption
                                       {
                                           id = g.Key.transmissionId,
                                           name = g.Key.transmissionName,
                                           count = g.Count()
                                       }).ToListAsync();

            var cities = await (from listing in activeListings
                                join city in db.GeographicCities on listing.cityId equals city.cityId
                                group listing by new { city.cityId, city.cityName } into g
                                orderby g.Count() descending
                                select new FilterOption
                                {
                                    id = g.Key.cityId,
                                    name = g.Key.cityName,
                                    count = g.Count()
                                }).Take(20).ToListAsync();

            var priceRange = await activeListings
                .Where(x => x.sellingPriceAsked > 0)
                .Select(x => x.sellingPriceAsked)
                .DefaultIfEmpty(0)
                .ToListAsync();

            var yearRange = await activeListings
                .Select(x => x.manufacturingYear)
                .ToListAsync();

            return (
                brands,
                models,
                categories,
                fuelTypes,
                transmissions,
                cities,
                priceRange.Any() ? priceRange.Min() : 0,
                priceRange.Any() ? priceRange.Max() : 0,
                yearRange.Any() ? yearRange.Min() : DateTime.Now.Year - 20,
                yearRange.Any() ? yearRange.Max() : DateTime.Now.Year
            );
        }














    }
}