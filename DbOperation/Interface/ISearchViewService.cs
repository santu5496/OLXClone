

using DbOperation.Models;
using DbOperation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.Interface
{
    public interface ISearchViewService
    {
        // Main Search Methods
        Task<CustomerCarSearchResultDto> SearchCarsAsync(CustomerCarSearchDto request);
        Task<CustomerCarDetailDto> GetCarDetailAsync(int listingId);

        // Home Page Data
        Task<HomePageDataDto> GetHomePageDataAsync();
        Task<List<CustomerCarDto>> GetFeaturedCarsAsync(int count = 10);
        Task<List<CustomerCarDto>> GetRecentCarsAsync(int count = 10);

        // Related Cars
        Task<List<CustomerCarDto>> GetSimilarCarsAsync(int listingId, int count = 5);
        Task<List<CustomerCarDto>> GetCarsByBrandAsync(int brandId, int count = 20);
        Task<List<CustomerCarDto>> GetCarsByPriceRangeAsync(decimal minPrice, decimal maxPrice, int count = 20);

        // Filter Options
        Task<List<FilterOption>> GetBrandsByPopularityAsync();
        Task<List<FilterOption>> GetModelsByBrandAsync(int brandId);
        Task<List<FilterOption>> GetCitiesByCarCountAsync();
        Task<List<FilterOption>> GetCategoriesAsync();
        Task<List<FilterOption>> GetFuelTypesAsync();
        Task<List<FilterOption>> GetTransmissionTypesAsync();

        // Popular & Trending
        Task<List<PopularSearchDto>> GetPopularSearchesAsync();
        Task<FilterStatisticsDto> GetFilterStatisticsAsync(CustomerCarSearchDto request);

        // Car Interactions
        Task<bool> RecordCarViewAsync(int listingId, string ipAddress = null);
        Task<bool> SubmitInquiryAsync(CustomerInquiryDto inquiry);
        Task<bool> AddToFavoritesAsync(int listingId, string userId);
        Task<bool> RemoveFromFavoritesAsync(int listingId, string userId);
        Task<List<CustomerCarDto>> GetUserFavoritesAsync(string userId);

        // Utility Methods
        Task<decimal> CalculateEMIAsync(decimal amount, decimal rate, int months);
        Task<List<string>> GetCarImagesAsync(int listingId);
        Task<List<CarFeatureItemDto>> GetCarFeaturesAsync(int listingId);

        // Statistics & Analytics
        Task<int> GetTotalActiveListingsAsync();
        Task<int> GetTotalVerifiedListingsAsync();
        Task<int> GetTotalFeaturedListingsAsync();
        Task<Dictionary<string, int>> GetCarCountByCategoryAsync();
        Task<Dictionary<string, int>> GetCarCountByPriceRangeAsync();
        Task<Dictionary<string, int>> GetCarCountByLocationAsync();

        // Advanced Search
        Task<List<CustomerCarDto>> QuickSearchAsync(string keyword, int? cityId = null, int count = 10);
        Task<List<string>> GetSearchSuggestionsAsync(string keyword);
        Task<CustomerCarSearchResultDto> GetCarsByFiltersAsync(
            int? brandId = null,
            int? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? cityId = null,
            int page = 1,
            int pageSize = 20);
    }
}