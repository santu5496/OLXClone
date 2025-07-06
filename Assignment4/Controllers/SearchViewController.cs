// =====================================================
// Part 4: Updated SearchViewController
// =====================================================

using DbOperation.Interface;
using DbOperation.Models;
using DbOperation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
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

        // Main search page view
        public async Task<IActionResult> SearchView(CustomerCarSearchDto searchRequest = null)
        {
            try
            {
                // If no search request, show default page with initial data
                if (searchRequest == null || string.IsNullOrEmpty(searchRequest.searchKeyword))
                {
                    var homePageData = await _searchViewService.GetHomePageDataAsync();
                    ViewBag.HomePageData = homePageData;

                    // Get filter options for dropdowns
                    ViewBag.PopularBrands = await _searchViewService.GetBrandsByPopularityAsync();
                    ViewBag.PopularCities = await _searchViewService.GetCitiesByCarCountAsync();
                    ViewBag.Categories = await _searchViewService.GetCategoriesAsync();
                    ViewBag.FuelTypes = await _searchViewService.GetFuelTypesAsync();
                    ViewBag.Transmissions = await _searchViewService.GetTransmissionTypesAsync();

                    return View("SearchView", new CustomerCarSearchResultDto());
                }

                // Perform search
                var searchResults = await _searchViewService.SearchCarsAsync(searchRequest);
                ViewBag.SearchRequest = searchRequest;

                return View("SearchView", searchResults);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error loading search page: {ex.Message}";
                return View("SearchView", new CustomerCarSearchResultDto());
            }
        }

        // Car detail page
        public async Task<IActionResult> CarDetail(int id)
        {
            try
            {
                // Record the view
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                await _searchViewService.RecordCarViewAsync(id, ipAddress);

                // Get car details
                var carDetail = await _searchViewService.GetCarDetailAsync(id);
                if (carDetail == null)
                {
                    return NotFound("Car not found");
                }

                return View("CarDetail", carDetail);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error loading car details: {ex.Message}";
                return View("Error");
            }
        }

        // AJAX endpoints for search functionality
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

        // Filter options endpoints
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

        // Statistics endpoints
        [HttpGet]
        public async Task<JsonResult> GetCarStatistics()
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

        // Error handling
        public IActionResult Error()
        {
            return View();
        }
    }
}