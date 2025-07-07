// =====================================================
// Updated SearchViewController for JavaScript Frontend
// =====================================================

using DbOperation.Interface;
using DbOperation.Models;
using DbOperation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assignment4.Controllers
{
    public class SearchViewController : Controller
    {
        private readonly ISearchViewService _searchViewService;

        public SearchViewController(ISearchViewService searchViewService)
        {
            _searchViewService = searchViewService;
        }

        // Main search page - no model binding, all data loaded via JavaScript
        public IActionResult SearchView()
        {
            return View();
        }

        // Car detail page - now returns the HTML view for JavaScript loading
        public IActionResult CarDetail(int? id)
        {
            // Return the HTML view that will load data via JavaScript
            return View();
        }

        // =====================================================
        // CRITICAL MISSING ENDPOINT - ADDED HERE
        // =====================================================

        /// <summary>
        /// Get car detail data for JavaScript frontend
        /// This is the endpoint your JavaScript is calling
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetCarDetail(int listingId)
        {
            try
            {
                // Record the view first
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                await _searchViewService.RecordCarViewAsync(listingId, ipAddress);

                // Get car details
                var carDetail = await _searchViewService.GetCarDetailAsync(listingId);

                if (carDetail == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Car not found"
                    });
                }

                return Json(new
                {
                    success = true,
                    data = carDetail
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error loading car details: {ex.Message}"
                });
            }
        }

        // Alternative endpoint that supports both path parameter and query parameter
        [HttpGet]
        [Route("SearchView/GetCarDetail/{listingId:int}")]
        public async Task<JsonResult> GetCarDetailByPath(int listingId)
        {
            return await GetCarDetail(listingId);
        }

        // =====================================================
        // PRIMARY ENDPOINTS FOR JAVASCRIPT FRONTEND
        // =====================================================

        // Get all filter data in one call (required by frontend)
        [HttpGet]
        public async Task<JsonResult> GetFilterData()
        {
            try
            {
                var filterData = new
                {
                    brands = await _searchViewService.GetBrandsByPopularityAsync(),
                    cities = await _searchViewService.GetCitiesByCarCountAsync(),
                    categories = await _searchViewService.GetCategoriesAsync(),
                    fuelTypes = await _searchViewService.GetFuelTypesAsync(),
                    transmissions = await _searchViewService.GetTransmissionTypesAsync()
                };

                return Json(new { success = true, data = filterData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Main search endpoint
        [HttpPost]
        public async Task<JsonResult> SearchCars([FromBody] CustomerCarSearchDto searchRequest)
        {
            try
            {
                var result = await _searchViewService.SearchCarsAsync(searchRequest);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Get car statistics for dashboard
        [HttpGet]
        public async Task<JsonResult> GetCarStatistics()
        {
            try
            {
                var totalActive = await _searchViewService.GetTotalActiveListingsAsync();
                var totalVerified = await _searchViewService.GetTotalVerifiedListingsAsync();
                var totalFeatured = await _searchViewService.GetTotalFeaturedListingsAsync();

                var statistics = new
                {
                    totalActive,
                    totalVerified,
                    totalFeatured
                };

                return Json(new { success = true, data = statistics });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Get models by brand (required for dependent dropdown)
        [HttpGet]
        public async Task<JsonResult> GetModelsByBrand(int brandId)
        {
            try
            {
                var result = await _searchViewService.GetModelsByBrandAsync(brandId);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Get search suggestions for autocomplete
        [HttpGet]
        public async Task<JsonResult> GetSearchSuggestions(string keyword)
        {
            try
            {
                var result = await _searchViewService.GetSearchSuggestionsAsync(keyword);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Submit customer inquiry
        [HttpPost]
        public async Task<JsonResult> SubmitInquiry([FromBody] CustomerInquiryDto inquiry)
        {
            try
            {
                var result = await _searchViewService.SubmitInquiryAsync(inquiry);
                return Json(new { success = result, message = result ? "Inquiry submitted successfully" : "Failed to submit inquiry" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Record car view for analytics
        [HttpPost]
        public async Task<JsonResult> RecordCarView(int listingId)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var result = await _searchViewService.RecordCarViewAsync(listingId, ipAddress);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Calculate EMI for cars
        [HttpGet]
        public async Task<JsonResult> CalculateEMI(decimal amount, decimal rate, int months)
        {
            try
            {
                var result = await _searchViewService.CalculateEMIAsync(amount, rate, months);
                return Json(new { success = true, emi = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // =====================================================
        // ADDITIONAL ENDPOINTS (OPTIONAL/FUTURE USE)
        // =====================================================

        // Featured cars
        [HttpGet]
        public async Task<JsonResult> GetFeaturedCars(int count = 10)
        {
            try
            {
                var result = await _searchViewService.GetFeaturedCarsAsync(count);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Recent cars
        [HttpGet]
        public async Task<JsonResult> GetRecentCars(int count = 10)
        {
            try
            {
                var result = await _searchViewService.GetRecentCarsAsync(count);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Similar cars (for car detail page)
        [HttpGet]
        public async Task<JsonResult> GetSimilarCars(int listingId, int count = 5)
        {
            try
            {
                var result = await _searchViewService.GetSimilarCarsAsync(listingId, count);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Quick search for autocomplete
        [HttpGet]
        public async Task<JsonResult> QuickSearch(string keyword, int? cityId = null, int count = 10)
        {
            try
            {
                var result = await _searchViewService.QuickSearchAsync(keyword, cityId, count);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Get filter statistics
        [HttpPost]
        public async Task<JsonResult> GetFilterStatistics([FromBody] CustomerCarSearchDto searchRequest)
        {
            try
            {
                var result = await _searchViewService.GetFilterStatisticsAsync(searchRequest);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Get cars by specific filters
        [HttpGet]
        public async Task<JsonResult> GetCarsByBrand(int brandId, int count = 20)
        {
            try
            {
                var result = await _searchViewService.GetCarsByBrandAsync(brandId, count);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCarsByPriceRange(decimal minPrice, decimal maxPrice, int count = 20)
        {
            try
            {
                var result = await _searchViewService.GetCarsByPriceRangeAsync(minPrice, maxPrice, count);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Get home page data
        [HttpGet]
        public async Task<JsonResult> GetHomePageData()
        {
            try
            {
                var result = await _searchViewService.GetHomePageDataAsync();
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Individual filter endpoints (kept for flexibility)
        [HttpGet]
        public async Task<JsonResult> GetPopularBrands()
        {
            try
            {
                var result = await _searchViewService.GetBrandsByPopularityAsync();
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetPopularCities()
        {
            try
            {
                var result = await _searchViewService.GetCitiesByCarCountAsync();
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCategories()
        {
            try
            {
                var result = await _searchViewService.GetCategoriesAsync();
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetFuelTypes()
        {
            try
            {
                var result = await _searchViewService.GetFuelTypesAsync();
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTransmissionTypes()
        {
            try
            {
                var result = await _searchViewService.GetTransmissionTypesAsync();
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Extended statistics endpoint
        [HttpGet]
        public async Task<JsonResult> GetDetailedStatistics()
        {
            try
            {
                var totalActive = await _searchViewService.GetTotalActiveListingsAsync();
                var totalVerified = await _searchViewService.GetTotalVerifiedListingsAsync();
                var totalFeatured = await _searchViewService.GetTotalFeaturedListingsAsync();
                var carsByCategory = await _searchViewService.GetCarCountByCategoryAsync();
                var carsByPriceRange = await _searchViewService.GetCarCountByPriceRangeAsync();
                var carsByLocation = await _searchViewService.GetCarCountByLocationAsync();

                var statistics = new
                {
                    totalActive,
                    totalVerified,
                    totalFeatured,
                    carsByCategory,
                    carsByPriceRange,
                    carsByLocation
                };

                return Json(new { success = true, data = statistics });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Advanced search with multiple filters
        [HttpPost]
        public async Task<JsonResult> AdvancedSearch([FromBody] CustomerCarSearchDto filters)
        {
            try
            {
                var result = await _searchViewService.GetCarsByFiltersAsync(
                    filters.brandId,
                    filters.categoryId,
                    filters.minPrice,
                    filters.maxPrice,
                    filters.cityId,
                    filters.page,
                    filters.pageSize);

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Batch operations for better performance
        [HttpPost]
        public async Task<JsonResult> GetMultipleFilterData([FromBody] string[] filterTypes)
        {
            try
            {
                var result = new Dictionary<string, object>();

                foreach (var filterType in filterTypes)
                {
                    switch (filterType.ToLower())
                    {
                        case "brands":
                            result["brands"] = await _searchViewService.GetBrandsByPopularityAsync();
                            break;
                        case "cities":
                            result["cities"] = await _searchViewService.GetCitiesByCarCountAsync();
                            break;
                        case "categories":
                            result["categories"] = await _searchViewService.GetCategoriesAsync();
                            break;
                        case "fueltypes":
                            result["fuelTypes"] = await _searchViewService.GetFuelTypesAsync();
                            break;
                        case "transmissions":
                            result["transmissions"] = await _searchViewService.GetTransmissionTypesAsync();
                            break;
                    }
                }

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Health check endpoint
        [HttpGet]
        public JsonResult HealthCheck()
        {
            return Json(new { success = true, message = "SearchViewController is healthy", timestamp = DateTime.UtcNow });
        }


        // Error handling
        public IActionResult Error()
        {
            return View();
        }
    }
}