// =====================================================
// Corrected SearchViewService Implementation
// =====================================================

using DbOperation.Interface;
using DbOperation.Models;
using DbOperation.ViewModels;
using DbOperation.ViewModels.DbOperation.ViewModels;
using iText.IO.Image;
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
                                                      brandId = listing.brandId,
                                                      brandName = brand.brandName,
                                                      modelId = listing.modelId,
                                                      modelName = model.modelName,
                                                      variantName = variant != null ? variant.variantName : null,
                                                      categoryId = listing.categoryId,
                                                      fuelTypeId = listing.fuelTypeId,
                                                      fuelType = fuelType.fuelTypeName,
                                                      transmissionId = listing.transmissionId,
                                                      transmission = transmission.transmissionName,
                                                      cityId = listing.cityId,
                                                      cityName = city.cityName,
                                                      stateId = listing.stateId,
                                                      stateName = state.stateName,
                                                      areaOrLocality = listing.areaOrLocality,
                                                      sellingPriceAsked = listing.sellingPriceAsked,
                                                      originalPurchasePrice = listing.originalPurchasePrice,
                                                      currentMarketPrice = listing.currentMarketPrice,
                                                      isPriceNegotiable = listing.isPriceNegotiable,
                                                      manufacturingYear = listing.manufacturingYear,
                                                      kilometersOnOdometer = listing.kilometersOnOdometer,
                                                      totalPreviousOwners = listing.totalPreviousOwners,
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
                                                      listingCreatedDate = (DateTime)listing.listingCreatedDate
                                                  };

                query = ApplyFilters(query, request);
                var totalCount = await query.CountAsync();
                query = ApplySorting(query, request);

                var skip = (request.page - 1) * request.pageSize;
                var results = await query.Skip(skip).Take(request.pageSize).ToListAsync();

                // Load images for all listings in current page
                var listingIds = results.Select(c => c.listingId).ToList();

                var imagesDict = await db.CarImages
                    .Where(img => listingIds.Contains(img.listingId))
                    .ToDictionaryAsync(img => img.listingId);

                var carDtos = new List<CustomerCarDto>();

                foreach (var car in results)
                {
                    var carDto = await MapToCustomerCarDto(car);

                    // Assign slot1ImageData converted to base64 string as PrimaryImage
                    if (imagesDict.TryGetValue(car.listingId, out var carImage))
                    {
                        if (carImage.slot1ImageData != null && carImage.slot1ImageData.Length > 0)
                        {
                            carDto.primaryImage = "data:image/jpeg;base64," + Convert.ToBase64String(carImage.slot1ImageData);
                        }
                        else
                        {
                            carDto.primaryImage = null; // or set a placeholder image path/url
                        }
                    }
                    else
                    {
                        carDto.primaryImage = null;
                    }

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

        private async Task<CustomerCarDto> MapToCustomerCarDto(CarListingDto car)
        {
            var carDto = new CustomerCarDto
            {
                listingId = car.listingId,
                title = car.listingTitle,
                brandName = car.brandName,
                modelName = car.modelName,
                variantName = car.variantName,
                year = car.manufacturingYear,
                fuelType = car.fuelType,
                transmission = car.transmission,
                kilometersOnOdometer = car.kilometersOnOdometer,
                totalPreviousOwners = car.totalPreviousOwners,
                cityName = car.cityName,
                stateName = car.stateName,
                areaOrLocality = car.areaOrLocality,
                sellingPrice = car.sellingPriceAsked,
                originalPrice = car.originalPurchasePrice,
                marketPrice = car.currentMarketPrice,
                isPriceNegotiable = car.isPriceNegotiable ?? false,
                exteriorRating = car.exteriorConditionRating,
                interiorRating = car.interiorConditionRating,
                engineRating = car.engineConditionRating,
                overallCondition = car.overallCondition,
                isVerified = car.isVerifiedListing ?? false,
                isFeatured = car.isFeaturedListing ?? false,
                isUrgentSale = car.isUrgentSale ?? false,
                hasAccidentHistory = car.hasAccidentHistory ?? false,
                sellerName = car.sellerName,
                sellerType = car.sellerType,
                sellerPhone = car.sellerPrimaryPhone,
                totalViews = car.totalViews,
                totalInquiries = car.totalInquiries,
                listingDate = car.listingCreatedDate
            };

            // Get images
            var images = await GetCarImagesAsync(car.listingId);
            carDto.totalImages = images.Count;


            // Get key features
            carDto.keyFeatures = await GetKeyFeaturesAsync(car.listingId);

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
            // Step 3: Load images from CarImages table
            var carImages = await db.CarImages.FirstOrDefaultAsync(i => i.listingId == listingId);
            // Step 1: Get the car listing with all required joined data
            var car = await (from listing in db.CarListings
                             join brand in db.CarBrands on listing.brandId equals brand.brandId
                             join model in db.CarModels on listing.modelId equals model.modelId
                             join fuel in db.FuelTypes on listing.fuelTypeId equals fuel.fuelTypeId
                             join trans in db.TransmissionTypes on listing.transmissionId equals trans.transmissionId
                             join city in db.GeographicCities on listing.cityId equals city.cityId
                             join state in db.GeographicStates on listing.stateId equals state.stateId
                             where listing.listingId == listingId
                             select new CustomerCarDetailDto
                             {
                                 listingId = listing.listingId,
                                 title = listing.listingTitle,

                                 brandName = brand.brandName,
                                 modelName = model.modelName,
                                 variantName = listing.sellerName,

                                 fuelType = fuel.fuelTypeName,
                                 transmission = trans.transmissionName,
                                 year = listing.manufacturingYear,
                                 kilometersOnOdometer = listing.kilometersOnOdometer,
                                 totalPreviousOwners = listing.totalPreviousOwners,

                                 cityName = city.cityName,
                                 stateName = state.stateName,
                                 areaOrLocality = listing.areaOrLocality,

                                 sellingPrice = listing.sellingPriceAsked,
                                 originalPrice = listing.originalPurchasePrice,
                                 marketPrice = listing.currentMarketPrice,
                                 isPriceNegotiable = listing.isPriceNegotiable ?? false,

                                 exteriorRating = listing.exteriorConditionRating,
                                 interiorRating = listing.interiorConditionRating,
                                 engineRating = listing.engineConditionRating,
                                // overallCondition = listing.overallCondition,

                                 isVerified = listing.isVerifiedListing ?? false,
                                 isFeatured = listing.isFeaturedListing ?? false,
                                 isUrgentSale = listing.isUrgentSale ?? false,
                                 hasAccidentHistory = listing.hasAccidentHistory ?? false,

                                 sellerName = listing.sellerName,
                                 sellerType = listing.sellerType,
                                 sellerPhone = listing.sellerPrimaryPhone,

                                 totalViews = listing.totalViews,
                                 totalInquiries = listing.totalInquiries,
                                 listingDate = listing.listingCreatedDate,
                                 primaryImage= carImages.slot1ImageData.ToString()

        }).FirstOrDefaultAsync();

            if (car == null)
                return null;

            // Step 2: Load additional details (from other sources if needed, like service, features, etc.)
            // Placeholder for optional data: car.featureGroups, car.similarCars, etc.

          
        

            if (carImages != null)
            {
                // Primary image from slot1
                if (carImages.slot1ImageData != null && carImages.slot1ImageData.Length > 0)
                {
                    car.primaryImage = "data:image/jpeg;base64," + Convert.ToBase64String(carImages.slot1ImageData);
                }

                var additionalImages = new List<string>();

                void AddSlotImage(byte[] imageData)
                {
                    if (imageData != null && imageData.Length > 0)
                    {
                        additionalImages.Add("data:image/jpeg;base64," + Convert.ToBase64String(imageData));
                    }
                }

                AddSlotImage(carImages.slot2ImageData);
                AddSlotImage(carImages.slot3ImageData);
                AddSlotImage(carImages.slot4ImageData);
                AddSlotImage(carImages.slot5ImageData);
                AddSlotImage(carImages.slot6ImageData);
                AddSlotImage(carImages.slot7ImageData);
                AddSlotImage(carImages.slot8ImageData);
                AddSlotImage(carImages.slot9ImageData);
                AddSlotImage(carImages.slot10ImageData);
                AddSlotImage(carImages.slot11ImageData);
                AddSlotImage(carImages.slot12ImageData);
                AddSlotImage(carImages.slot13ImageData);
                AddSlotImage(carImages.slot14ImageData);
                AddSlotImage(carImages.slot15ImageData);

                car.additionalImages = additionalImages;
                car.totalImages = (string.IsNullOrEmpty(car.primaryImage) ? 0 : 1) + additionalImages.Count;
            }

            return car;
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

        // Fixed MapToCustomerCarDto overload for anonymous types
        private async Task<CustomerCarDto> MapToCustomerCarDto(dynamic car)
        {
            var carDto = new CustomerCarDto
            {
                listingId = car.listingId,
                title = car.listingTitle,
                brandName = car.brandName,
                modelName = car.modelName,
                year = car.manufacturingYear,
                fuelType = car.fuelType,
                transmission = car.transmission,
                kilometersOnOdometer = car.kilometersOnOdometer,
                totalPreviousOwners = car.totalPreviousOwners,
                cityName = car.cityName,
                stateName = car.stateName,
                areaOrLocality = car.areaOrLocality,
                sellingPrice = car.sellingPriceAsked,
                originalPrice = car.originalPurchasePrice,
                isPriceNegotiable = false, // Default value since not available in anonymous type
                isVerified = car.isVerifiedListing ?? false,
                isFeatured = car.isFeaturedListing ?? false,
                hasAccidentHistory = car.hasAccidentHistory ?? false,
                sellerName = car.sellerName,
                sellerType = car.sellerType,
                sellerPhone = car.sellerPrimaryPhone,
                totalViews = car.totalViews,
                listingDate = car.listingCreatedDate
            };

            // Get images
            var images = await GetCarImagesAsync(car.listingId);
            carDto.totalImages = images.Count;

            // Get key features
            carDto.keyFeatures = await GetKeyFeaturesAsync(car.listingId);

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
            // Check each slot for image data and convert to base64
            var imageSlots = new byte[][]
            {
                carImages.slot1ImageData,
                carImages.slot2ImageData,
                carImages.slot3ImageData,
                carImages.slot4ImageData,
                carImages.slot5ImageData,
                carImages.slot6ImageData,
                carImages.slot7ImageData,
                carImages.slot8ImageData,
                carImages.slot9ImageData,
                carImages.slot10ImageData,
                carImages.slot11ImageData,
                carImages.slot12ImageData,
                carImages.slot13ImageData,
                carImages.slot14ImageData,
                carImages.slot15ImageData
            };

            foreach (var imageData in imageSlots)
            {
                if (imageData != null && imageData.Length > 0)
                {
                    // Convert byte array to base64 string
                    var base64String = Convert.ToBase64String(imageData);
                    images.Add($"data:image/jpeg;base64,{base64String}");
                }
            }
        }

        return images;
    }
    catch (Exception ex)
    {
        // Log error if you have logging
        Console.WriteLine($"Error loading images for listing {listingId}: {ex.Message}");
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

        private async Task<CarFeaturesWithImageDto> GetCarFeatureGroupsAsync(int listingId)
        {
            using var db = new Assignment4Context(_dbConn);

            try
            {
                // 1. Load Features
                var features = await (from lf in db.CarListingFeatures
                                      join f in db.CarFeaturesList on lf.featureId equals f.featureId
                                      where lf.listingId == listingId
                                      select new
                                      {
                                          categoryName = f.featureCategory ?? "Other",
                                          featureName = f.featureDisplayName ?? f.featureName,
                                          isAvailable = lf.isFeatureAvailable ?? false,
                                          condition = lf.featureCondition
                                      }).ToListAsync();

                var featureGroups = features
                    .GroupBy(x => x.categoryName)
                    .Select(g => new CarFeatureGroupDto
                    {
                        categoryName = g.Key,
                        features = g.Select(f => new CarFeatureItemDto
                        {
                            featureName = f.featureName,
                            isAvailable = f.isAvailable,
                            condition = f.condition
                        }).ToList()
                    }).ToList();

                // 2. Load primary image from first non-null slot
                var carImage = await db.CarImages
                    .Where(img => img.listingId == listingId)
                    .FirstOrDefaultAsync();

                byte[] imageBytes = carImage?.slot1ImageData
                    ?? carImage?.slot2ImageData
                    ?? carImage?.slot3ImageData
                    ?? carImage?.slot4ImageData
                    ?? carImage?.slot5ImageData
                    ?? carImage?.slot6ImageData
                    ?? carImage?.slot7ImageData
                    ?? carImage?.slot8ImageData
                    ?? carImage?.slot9ImageData
                    ?? carImage?.slot10ImageData
                    ?? carImage?.slot11ImageData
                    ?? carImage?.slot12ImageData
                    ?? carImage?.slot13ImageData
                    ?? carImage?.slot14ImageData
                    ?? carImage?.slot15ImageData;

                string base64Image = imageBytes != null
                    ? $"data:image/jpeg;base64,{Convert.ToBase64String(imageBytes)}"
                    : null;

                // 3. Return combined result
                return new CarFeaturesWithImageDto
                {
                    Base64Image = base64Image,
                    FeatureGroups = featureGroups
                };
            }
            catch
            {
                return new CarFeaturesWithImageDto(); // empty fallback
            }
        }

        public class CarFeaturesWithImageDto
        {
            public string? Base64Image { get; set; } // or use byte[] ImageData;
            public List<CarFeatureGroupDto> FeatureGroups { get; set; } = new List<CarFeatureGroupDto>();

            public static implicit operator List<object>(CarFeaturesWithImageDto v)
            {
                throw new NotImplementedException();
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

        // Overloaded method for CustomerCarDetailDto
        private List<string> GenerateBadges(CustomerCarDetailDto car)
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

        // Overloaded method for CustomerCarDetailDto
        private string GeneratePriceLabel(CustomerCarDetailDto car)
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
                // Fixed: Use ToListAsync() to execute query before client-side operations
                var prices = await db.CarListings
                    .Where(x => x.listingStatus == "Active" && x.sellingPriceAsked > 0)
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

        // Fixed GetFilterOptionsAsync method
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

            // Fixed: Execute query and get results, then calculate min/max
            var priceData = await activeListings
                .Where(x => x.sellingPriceAsked > 0)
                .Select(x => x.sellingPriceAsked)
                .ToListAsync();

            var yearData = await activeListings
                .Select(x => x.manufacturingYear)
                .ToListAsync();

            return (
                brands,
                models,
                categories,
                fuelTypes,
                transmissions,
                cities,
                priceData.Any() ? priceData.Min() : 0,
                priceData.Any() ? priceData.Max() : 0,
                yearData.Any() ? yearData.Min() : DateTime.Now.Year - 20,
                yearData.Any() ? yearData.Max() : DateTime.Now.Year
            );
        }
    }
}